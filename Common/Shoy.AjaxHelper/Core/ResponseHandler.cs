using System.Web;
using System.Web.SessionState;
using Shoy.AjaxHelper.Model;
using Shoy.Utility.Extend;

namespace Shoy.AjaxHelper.Core
{
    internal class ResponseHandler : IHttpHandler, IRequiresSessionState
    {
        /// <summary>
        /// 需要执行方法的一些路径信息
        /// </summary>
        public MethodPathInfo CurrentMethodPathInfo { get; private set; }

        /// <summary>
        /// 构造方法  主要是初始化请求方法的一些路径信息 比如 空间 类  方法名
        /// </summary>
        ///<param name="virtualPath">请求的一个虚拟路径</param>
        public ResponseHandler(string virtualPath)
        {
            CurrentMethodPathInfo = UrlHelper.GetMethodPathInfo(virtualPath);
        }

        public void ProcessRequest(HttpContext context)
        {
            //暂时都是输出json格式
            context.Response.ContentType = "application/json";

            var result = new AjaxResult();
            try
            {
                var methodHelper = new MethodHelper(context, CurrentMethodPathInfo);
                var method = methodHelper.GetMethod();
                if (method != null)
                {
                    result.state = 1;
                    var rt = (methodHelper.ExecutinonMethod(method) ?? "");
                    if (rt is string)
                        result.result = rt;
                    else
                        result.result = rt.ToJson();

                }
                else
                {
                    result.state = 0;
                    result.msg = "方法调用失败！";
                }
                var callback = context.Request["callback"];
                var html = result.state == 1 ? result.result : result.ToJson();
                if(callback.IsNotNullOrEmpty())
                {
                    context.Response.ContentType = "application/x-javascript";
                    html = callback + "(" + html + ")";
                }
                context.Response.Write(html);
            }
            catch (AjaxException ex)
            {
                context.Response.Write(ex.Message);
            }
        }

        public bool IsReusable
        {
            get { return false; }
        }
    }
}
