using System.Web.Mvc;

namespace Shoy.Web
{
    /// <summary> Controller基类 </summary>
    public abstract class DController : Controller
    {
        protected System.Web.HttpContext CurrentContext
        {
            get { return System.Web.HttpContext.Current; }
        }

        protected string RawUrl
        {
            get
            {
                if (CurrentContext == null) return string.Empty;
                return string.Format("http://{0}{1}", CurrentContext.Request.ServerVariables["HTTP_HOST"],
                    CurrentContext.Request.RawUrl);
            }
        }
    }
}
