using System.Configuration;
using System.Web.Configuration;
using System.Web.Mvc;
using Shoy.Core;
using Shoy.Utility;
using Shoy.Utility.Logging;

namespace Shoy.Web.Filters
{
    /// <summary> 得一平台异常处理特性 </summary>
    public class DExceptionAttribute : HandleErrorAttribute
    {
        private readonly ILogger _logger = LogManager.Logger<DExceptionAttribute>();
        private DExceptionAttribute()
        { }
        public static DExceptionAttribute Instance
        {
            get
            {
                return Singleton<DExceptionAttribute>.Instance ??
                       (Singleton<DExceptionAttribute>.Instance = new DExceptionAttribute());
            }
        }

        public override void OnException(ExceptionContext filterContext)
        {
            if (!filterContext.ExceptionHandled)
            {
                var ex = filterContext.Exception;
                //记录日志
                _logger.Error(ex.Message, ex);

                //读取配置，是否跳转
                var customErrors = (CustomErrorsSection)ConfigurationManager.GetSection("system.web/customErrors");
                if (customErrors != null && customErrors.Mode == CustomErrorsMode.On)
                {
                    //转向
                    filterContext.ExceptionHandled = true;
                    filterContext.Result = new RedirectResult(Consts.Config.MainUrl + "/500");
                }
            }
            base.OnException(filterContext);
        }
    }
}
