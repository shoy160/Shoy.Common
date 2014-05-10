using System;
using Shoy.AjaxHelper.Model;
using Shoy.Utility.Extend;

namespace Shoy.AjaxHelper
{
    /// <summary>
    /// Ajax请求身份验证
    /// Level=8888
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class AjaxAuth : AttrBase
    {
        private const string ShoyKey = "shoy_token";

        public AjaxAuth()
        {
            Level = 8888;
        }

        public override bool IsValidate()
        {
            if (base.IsValidate())
            {
                var token = CurrentHttpRequest.WebParameters[ShoyKey];
                if (token.IsNullOrEmpty())
                    throw new AjaxException("口令不能为空！");
                return CheckAuth(token, MethodPath.MethodKey);
            }
            return false;
        }

        private static bool CheckAuth(string token, string methodKey)
        {
            return token.IsNotNullOrEmpty() && methodKey.IsNotNullOrEmpty();
        }
    }
}
