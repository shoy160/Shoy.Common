
using System;
using com.dayeasy.service.paper.api;
using Damai.Dubbo.Client;
using Shoy.Utility.Helper;

namespace Shoy.DubboWeb.Models
{
    public class DubboHelper
    {
        private readonly string _consumerName;
        private readonly string _dubboServer;
        public DubboHelper()
        {
            _consumerName = ConfigHelper.GetAppSetting<string>(supressKey: "dubbo_name");
            _dubboServer = ConfigHelper.GetAppSetting<string>(supressKey: "dubbo_server");
        }

        public static DubboHelper Instance
        {
            get { return new DubboHelper(); }
        }

        public TV DubboAction<T, TV>(Func<T, TV> action)
            where T : IDubboService
        {
            //"zookeeper", "zookeeper://192.168.10.14:2181", 2181
            var client = new DubboClient(_consumerName, _dubboServer);
            using (var reference = new ServiceReference<T>
            {
                DubboClient = client,
                Registry = client.Registry,
                Timeout = 2 * 60
            })
            {
                return action(reference.Get());
            }
        }
    }
}