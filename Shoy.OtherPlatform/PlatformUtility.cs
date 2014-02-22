using System.Collections.Specialized;
using System.Web;
using System.Xml.Linq;
using Shoy.OtherPlatform.Entity;
using Shoy.Utility;
using Shoy.Utility.Extend;

namespace Shoy.OtherPlatform
{
    internal static class PlatformUtility
    {
        private static string _xmlPath = string.Empty;

        internal static AppInfo GetKeyInfos(PlatformType p)
        {
            _xmlPath = Utils.GetCurrentDir() + "/App_Data/xml/ShoyApp.config";
            XElement root = XElement.Load(_xmlPath);
            XElement xel = root.Element(p.ToString());
            AppInfo app = null;
            if (xel != null)
            {
                app = new AppInfo();
                XAttribute id = xel.Attribute("pid"),
                           key = xel.Attribute("key");
                app.AppId = (id == null ? "" : id.Value);
                app.Key = (key == null ? "" : key.Value);
            }
            return app;
        }

        internal static AppInfo GetAppInfoFromCaching(PlatformType p)
        {
            System.Web.Caching.Cache m = HttpRuntime.Cache;
            if (m[p.ToString()] == null)
            {
                System.Web.Caching.Cache c = HttpRuntime.Cache;
                AppInfo f = GetKeyInfos(p);
                c.Insert(p.ToString(), f, new System.Web.Caching.CacheDependency(_xmlPath));
            }
            return m[p.ToString()] as AppInfo;
        }

        internal static NameValueCollection GetContext(string str)
        {
            var val = new NameValueCollection();
            try
            {
                if (str.IsNullOrEmpty()) return val;
                if (str.As<IRegex>().IsMatch("^\\{.*\\}$"))
                {
                    var qs = str.Trim(new[] {'{', '}'}).Split(',');
                    foreach (var s in qs)
                    {
                        var q = s.Split(':');
                        if (q.Length >= 2)
                        {
                            if (q.Length > 2)
                            {
                                for (int i = 2; i < q.Length; i++)
                                {
                                    q[1] += ":" + q[i];
                                }
                            }
                            val.Add(q[0].Trim('"'), q[1].Trim('"'));
                        }
                    }
                }
                else
                {
                    var qs = str.Split('&');
                    foreach (var t in qs)
                    {
                        if (t.IsNotNullOrEmpty())
                        {
                            var q = t.Split('=');
                            val.Add(q[0], Utils.UrlDecode(q[1]));
                        }
                    }
                }
            }
            catch{}
            return val;
        }
    }
}
