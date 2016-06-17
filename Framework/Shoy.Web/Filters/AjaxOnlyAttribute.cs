using System;
using System.Web.Mvc;
using Shoy.Utility;
using Shoy.Web.ActionResults;

namespace Shoy.Web.Filters
{
    /// <summary> 异步方法过滤 </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AjaxOnlyAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAjaxRequest())
                return;
            filterContext.Result = DJson.Json(DResult.Error("该方法只允许Ajax调用！"));
        }
    }
}
