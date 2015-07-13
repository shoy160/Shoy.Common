
using System.Linq;
using System.Reflection;
using Shoy.ThirdPlatform.Entity;
using Shoy.ThirdPlatform.Entity.Config;
using Shoy.Utility.Config;
using Shoy.Utility.Extend;

namespace Shoy.ThirdPlatform.Api
{
    public abstract class HelperBase
    {
        protected static Platform Config { get; private set; }

        internal static HelperBase GetInstance(PlatformType type)
        {
            return GetInstance(type.ToString());
        }

        internal static HelperBase GetInstance(string type)
        {
            HelperBase instance;
            if (!string.IsNullOrEmpty(type))
            {
                var ass = Assembly.GetCallingAssembly();
                instance =
                    (HelperBase)
                        ass.CreateInstance("Shoy.OtherPlatform.Platform." + type);
                if (instance != null)
                    instance.Init();
            }
            else
                instance = null;
            return instance;
        }

        /// <summary> 初始化配置文件 </summary>
        protected abstract void Init();

        /// <summary> 加载平台配置文件 </summary>
        /// <param name="type"></param>
        protected void LoadPlatform(PlatformType type)
        {
            if (Config != null)
                return;
            var config = ConfigUtils<PlatformConfig>.Instance().Get();
            Config = (config == null
                ? new Platform()
                : config.Platforms.FirstOrDefault(t => t.PlatType == type.GetValue())
                  ?? new Platform());
        }

        /// <summary> 获取登录链接 </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        public abstract string LoginUrl(string callback);

        /// <summary> 获取登录用户 </summary>
        /// <param name="callbackUrl"></param>
        /// <returns></returns>
        public abstract UserBase Login(string callbackUrl);
    }
}
