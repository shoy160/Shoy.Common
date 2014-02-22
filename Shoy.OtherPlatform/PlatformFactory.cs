using System.Reflection;
using System.Web;
using Shoy.OtherPlatform.Entity;


namespace Shoy.OtherPlatform
{
    public abstract class PlatformFactory
    {
        public static PlatformFactory GetInstance(PlatformType type)
        {
            return GetInstance(type.ToString());
        }

        public static PlatformFactory GetInstance(string type)
        {
            PlatformFactory instance;
            if (!string.IsNullOrEmpty(type))
            {
                var ass = Assembly.Load("Shoy.OtherPlatform");
                instance =
                    (PlatformFactory)
                    ass.CreateInstance("Shoy.OtherPlatform.Platform." + type);
            }
            else
                instance = null;
            return instance;
        }

        public abstract string CreateLoginUrl(string callBackUrl);

        public abstract UserInfo GetUserInfo(HttpContext httpContent, string callBackUrl);
    }
}
