
namespace Shoy.Web.Filters
{
    /// <summary> 角色身份过滤器 </summary>
    public class RoleAuthorizeAttribute : DAuthorizeAttribute
    {
        //        private readonly string _redirect;
        //        private readonly UserRole _userRole;

        //        public RoleAuthorizeAttribute(UserRole role, string redirect)
        //        {
        //            _userRole = role;
        //            _redirect = redirect;
        //        }
        //
        //        protected override bool AuthorizeCore(HttpContextBase httpContext)
        //        {
        //            var result = base.AuthorizeCore(httpContext);
        //            return result && HasRole(_userRole);
        //        }
        //
        //        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        //        {
        //            base.HandleUnauthorizedRequest(filterContext);
        //            if (User == null)
        //                return;
        //            filterContext.Result = new RedirectResult(_redirect);
        //        }
    }
}
