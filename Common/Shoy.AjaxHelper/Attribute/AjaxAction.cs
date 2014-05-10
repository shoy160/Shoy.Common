using System;
using System.Collections.Specialized;
using System.Web;
using Shoy.Utility.Extend;
using Shoy.AjaxHelper.Model;

namespace Shoy.AjaxHelper
{
    /// <summary>
    /// Ajax请求方法特征,通过反射查找
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class AjaxAction : AttrBase
    {
        public RequestType CurrentRequestType { get; private set; }

        public ContentType ContentType { get; private set; }

        public AjaxAction()
        {
            CurrentRequestType = RequestType.Get;
            Level = 9999;
        }

        public AjaxAction(RequestType requestType)
        {
            CurrentRequestType = requestType;
            Level = 9999;
        }

        public override bool IsValidate()
        {
            if (base.IsValidate())
            {
                var currentType = CurrentRequestType.ToString().ToUpper();
                if (CurrentRequestType == RequestType.All ||
                    CurrentHttpRequest.Context.Request.RequestType == currentType)
                    return true;
            }
            throw new AjaxException("此方法无法用{0}方式进行访问".FormatWith(CurrentHttpRequest.Context.Request.RequestType));
        }

        public NameValueCollection GetWebParameters(HttpContext context)
        {
            if (context == null)
            {
                throw new AjaxException("请求的上下文为空");
            }
            NameValueCollection webParameters;
            switch (CurrentRequestType.ToString().ToUpper())
            {
                case "POST":
                    webParameters = context.Request.Form;
                    break;
                case "GET":
                    webParameters = context.Request.QueryString;
                    break;
                default:
                    webParameters = context.Request.Params;
                    break;
            }
            return webParameters;
        }
    }
}
