﻿
using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using Shoy.Core.Dependency;
using Shoy.Core.Reflection;
using Shoy.Utility;
using Shoy.Utility.Helper;
using Shoy.Utility.Logging;

namespace Shoy.Core.Wcf
{
    public class WcfHelper
    {
        private readonly ILogger _logger = LogManager.Logger<WcfHelper>();
        private WcfHelper() { }
        public static WcfHelper Instance
        {
            get { return (Singleton<WcfHelper>.Instance ?? (Singleton<WcfHelper>.Instance = new WcfHelper())); }
        }

        private string WcfHost
        {
            get { return ConfigHelper.GetAppSetting(defaultValue: string.Empty); }
        }

        public ITypeFinder TypeFinder { private get; set; }
        public IIocManager IocManager { private get; set; }

        public void StartService()
        {
            var types =
                TypeFinder.Find(
                    t => t.IsInterface && t != typeof(IWcfService) && typeof(IWcfService).IsAssignableFrom(t));
            foreach (var type in types)
            {
                var resolve = IocManager.Resolve(type);
                OpenService(type, resolve.GetType());
            }
        }

        private void OpenService(Type interfaceType, Type classType)
        {
            var uri = new Uri(WcfHost + "/" + interfaceType.Name);
            var host = new ServiceHost(classType);
            host.AddServiceEndpoint(interfaceType, new WSHttpBinding(), uri);
            if (host.Description.Behaviors.Find<ServiceMetadataBehavior>() == null)
            {
                var behavior = new ServiceMetadataBehavior
                {
                    HttpGetEnabled = true,
                    HttpGetUrl = uri
                };
                host.Description.Behaviors.Add(behavior);
            }
            host.Opened += delegate
            {
                _logger.Info("service:{0} 已启动！", uri);
            };

            host.Open();
        }

        /// <summary> 调用WCF服务 </summary>
        /// <typeparam name="TContract">需要调用的WCF契约</typeparam>
        /// <param name="action">需要调用的WCF方法</param>
        public void Call<TContract>(Action<TContract> action)
        {
            var address = WcfHost + "/" + typeof(TContract).Name;
            var factory = new ChannelFactory<TContract>(new WSHttpBinding(), new EndpointAddress(new Uri(address)));
            var channel = factory.CreateChannel();
            var client = ((IClientChannel)channel);

            try
            {
                client.Open();
                action(channel);
                client.Close();
            }
            catch
            {
                client.Abort();
            }
        }
    }
}
