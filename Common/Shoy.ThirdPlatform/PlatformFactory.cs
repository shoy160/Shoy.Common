
using Shoy.ThirdPlatform.Api;
using Shoy.ThirdPlatform.Entity.Config;

namespace Shoy.ThirdPlatform
{
    public class PlatformFactory
    {
        public static HelperBase GetInstance(PlatformType type)
        {
            return HelperBase.GetInstance(type);
        }

        public static HelperBase GetInstance(string type)
        {
            return HelperBase.GetInstance(type);
        }
    }
}
