using System.Linq;
using System.Reflection;
using Shoy.ThirdPlatform.Entity;
using Shoy.ThirdPlatform.Entity.Config;
using Shoy.Utility;
using Shoy.Utility.Config;
using Shoy.Utility.Extend;

namespace Shoy.ThirdPlatform.Helper
{
    public abstract class HelperBase
    {
        protected string Callback { get; set; }
        protected Platform Config { get; private set; }

        internal static HelperBase GetInstance(string type)
        {
            HelperBase instance;
            if (!string.IsNullOrEmpty(type))
            {
                var ass = Assembly.GetExecutingAssembly();
                instance =
                    (HelperBase)
                        ass.CreateInstance(string.Format("{0}.Helper.{1}", ass.GetName().Name, type));
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
            if (config != null)
            {
                Callback = config.Callback;
                Config = config.Platforms.FirstOrDefault(t => t.PlatType == type.GetValue())
                         ?? new Platform();
            }
            else
            {
                Config = new Platform();
            }
        }

        /// <summary> 获取登录链接 </summary>
        /// <returns></returns>
        public abstract string LoginUrl();

        /// <summary> 获取登录用户 </summary>
        /// <returns></returns>
        public abstract DResult<UserResult> User();
    }
}
