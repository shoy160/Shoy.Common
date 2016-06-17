using System;
using System.Text;
using System.Web.Mvc;
using Shoy.Utility.Extend;
using Shoy.Utility.Helper;

namespace Shoy.Web.ActionResults
{
    public class ScriptResult : ActionResult
    {
        private readonly object _data;
        private string _method;
        public ScriptResult(object data, string method = null)
        {
            _data = data;
            _method = method;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            var response = context.HttpContext.Response;
            if (string.IsNullOrWhiteSpace(_method))
                _method = context.HttpContext.Request["callback"];
            response.ContentType = "text/html";
            response.ContentEncoding = Encoding.UTF8;
            response.Write("<script>{0}({1});</script>".FormatWith(_method,
                JsonHelper.ToJson(_data, NamingType.CamelCase)));
        }
    }
}
