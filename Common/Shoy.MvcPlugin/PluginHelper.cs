using System.Web;

namespace Shoy.MvcPlugin
{
    /// <summary>
    /// 插件帮助类
    /// </summary>
    public class PluginHelper
    {
        private static AspNetHostingPermissionLevel? _trustLevel = null;

        public static AspNetHostingPermissionLevel GetTrustLevel()
        {
            if (!_trustLevel.HasValue)
            {
                //set minimum
                _trustLevel = AspNetHostingPermissionLevel.None;

                //determine maximum
                foreach (AspNetHostingPermissionLevel trustLevel in
                    new[]
                        {
                            AspNetHostingPermissionLevel.Unrestricted,
                            AspNetHostingPermissionLevel.High,
                            AspNetHostingPermissionLevel.Medium,
                            AspNetHostingPermissionLevel.Low,
                            AspNetHostingPermissionLevel.Minimal
                        })
                {
                    try
                    {
                        new AspNetHostingPermission(trustLevel).Demand();
                        _trustLevel = trustLevel;
                        break; //we've set the highest permission we can
                    }
                    catch (System.Security.SecurityException)
                    {
                    }
                }
            }
            return _trustLevel.Value;
        }
    }
}
