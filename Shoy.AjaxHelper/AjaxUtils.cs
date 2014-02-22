using System.Web;
using Shoy.AjaxHelper.Core;
using Shoy.AjaxHelper.Model;

namespace Shoy.AjaxHelper
{
    public static class AjaxUtils
    {
        public static object ExecutinonMethod(string ass, string className, string method, HttpContext context)
        {
            var result = new AjaxResult();
            try
            {
                var info = new MethodPathInfo
                               {
                                   Assembly = ass,
                                   ClassName = className,
                                   MethodName = method
                               };
                var helper = new MethodHelper(context, info);
                var m = helper.GetMethod();
                if (method != null)
                {
                    result.state = 1;
                    var rt = (helper.ExecutinonMethod(m) ?? "");
                    if (rt is string)
                        result.result = rt;
                    else
                        result.result = rt;

                }
                else
                {
                    result.state = 0;
                    result.msg = "方法调用失败！";
                }
                return result.state == 1 ? result.result : result;
            }
            catch (AjaxException ex)
            {
                return ex.GetResult();
            }
        }

        public static string ShowMethods()
        {
            return MethodHelper.ShowMethods();
        }
    }
}
