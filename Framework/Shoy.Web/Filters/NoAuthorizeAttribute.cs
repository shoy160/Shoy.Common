using System;

namespace Shoy.Web.Filters
{
    /// <summary> Action 过滤掉Controller 上的权限过滤器 </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class NoAuthorizeAttribute : Attribute
    {
    }
}