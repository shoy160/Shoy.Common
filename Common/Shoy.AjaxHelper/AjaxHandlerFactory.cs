using System.Web;
using Shoy.AjaxHelper.Core;

namespace Shoy.AjaxHelper
{
    public class AjaxHandlerFactory : IHttpHandlerFactory
    {
        public IHttpHandler GetHandler(HttpContext context, string requestType, string url, string pathTranslated)
        {
            return new ResponseHandler(url);
        }

        public void ReleaseHandler(IHttpHandler handler)
        {
        }
    }
}
