using System.IO;
using System.Web;
using System.Web.Mvc;
using Shoy.Utility;
using Shoy.Utility.Helper;
using Shoy.Web.ActionResults;

namespace Shoy.Web
{
    /// <summary> Controller基类 </summary>
    public abstract class DController : Controller
    {
        protected HttpContext CurrentContext
        {
            get { return System.Web.HttpContext.Current; }
        }

        /// <summary> 原始Url </summary>
        protected string RawUrl
        {
            get
            {
                return Utils.RawUrl();
            }
        }

        /// <summary> 从Request的InputStream中加载实体 </summary>
        protected T FromBody<T>()
        {
            if (CurrentContext == null) return default(T);
            var input = CurrentContext.Request.InputStream;
            input.Seek(0, SeekOrigin.Begin);
            using (var stream = new StreamReader(input))
            {
                var body = stream.ReadToEnd();
                return JsonHelper.Json<T>(body, NamingType.CamelCase);
            }
        }

        /// <summary> 小驼峰式JsonResult </summary>
        protected ActionResult DeyiJson(object data, bool denyGet = false)
        {
            return DJson.Json(data, denyGet, namingType: NamingType.CamelCase);
        }
    }
}
