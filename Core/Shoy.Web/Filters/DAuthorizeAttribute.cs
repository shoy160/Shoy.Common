using System.Linq;
using System.Web;
using System.Web.Mvc;
using Shoy.Core;
using Shoy.Utility.Extend;
using Shoy.Utility.Helper;

namespace Shoy.Web.Filters
{
    /// <summary> 平台基础身份过滤器 </summary>
    public class DAuthorizeAttribute : AuthorizeAttribute
    {
        // 有权限的用户
        public string AllowUser { get; set; }

        // 授权失败时呈现的视图名称
        public string View { get; set; }

        #region 请求授权时执行

        /// <summary>
        /// 请求授权时执行
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var actionDescriptor = filterContext.ActionDescriptor;
            var noAuthorizeAttributes = actionDescriptor.GetCustomAttributes(typeof(NoAuthorizeAttribute), true);
            if (!noAuthorizeAttributes.Any())
            {
                base.OnAuthorization(filterContext);
            }
        }

        #endregion

        #region 自定义授权检查

        /// <summary>
        /// 自定义授权检查
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext == null)
                return false;
            //用户登录检查
            //获取token
            var token = "token".QueryOrForm(string.Empty);
            if (string.IsNullOrWhiteSpace(token))
            {
                token = CookieHelper.GetValue(Consts.UserCookieName);
            }
            var comefrom = "comefrom".QueryOrForm(0);
            //获取当前登录的用户信息
            //            User = UserCache.User(token, (byte)comefrom);
            //            if (User == null)
            //                return false;
            //            UserRoles = facade.GetUserRoles(User.Role);
            return true;
        }

        #endregion

        #region 处理授权失败的处理

        /// <summary>
        /// 处理授权失败的处理
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            base.HandleUnauthorizedRequest(filterContext);
//            if (User != null) return;
            var req = filterContext.HttpContext.Request;
            var rawUrl = req.RawUrl.TrimStart('/');
            string url = Consts.Config.LoginUrl + "?return_url={0}";
            string returnUrl = string.Format("http://{0}/{1}", req.ServerVariables["HTTP_HOST"],
                rawUrl);
            //异步请求
            if (req.IsAjaxRequest())
            {
                if (req.UrlReferrer != null)
                    returnUrl = req.UrlReferrer.AbsoluteUri;
                url = string.Format(url,
                    filterContext.HttpContext.Server.UrlEncode(returnUrl));
                //                var javascriptResult = new JavaScriptResult
                //                {
                //                    Script = "<script>window.top.location.href='" + url + "';</script>"
                //                };
                var result = new JsonResult
                {
                    Data = new { login = true, url },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
                filterContext.Result = result;
                return;
            }
            if (returnUrl.Contains(Consts.Config.MainUrl) && string.IsNullOrWhiteSpace(rawUrl))
                url = Consts.Config.LoginUrl;
            else
                url = string.Format(url,
                    filterContext.HttpContext.Server.UrlEncode(returnUrl));
            filterContext.Result = new RedirectResult(url);
        }

        #endregion

    }
}