using System.Web.Mvc;
using Shoy.Utility.Helper;

namespace Shoy.Web.ActionResults
{
    public class JsonpResult : ActionResult
    {
        private readonly object _result;
        private readonly string _callback;

        public JsonpResult(object result, string callback = null)
        {
            _result = result;
            _callback = string.IsNullOrWhiteSpace(callback) ? "callback" : callback;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var jsonp = _callback + "(" + JsonHelper.ToJson(_result, NamingType.CamelCase) + ")";
            context.HttpContext.Response.ContentType = "application/json";
            context.HttpContext.Response.Write(jsonp);
            context.HttpContext.Response.End();
        }
    }
}
