using System;
using System.Linq;

namespace Shoy.AjaxHelper
{
    /// <summary>
    /// Ajax请求缓存特征
    /// </summary>
    [AttributeUsage(AttributeTargets.Method,AllowMultiple = false,Inherited = false)]
    public class AjaxCache:AttrBase
    {
        /// <summary>
        /// jQuery缓存头,不能改
        /// </summary>
        private const string CacheName = "If-Modified-Since";
        private readonly int _seconds;

        /// <summary>
        /// 设置缓存秒数
        /// </summary>
        /// <param name="seconds"></param>
        public AjaxCache(int seconds)
        {
            _seconds = seconds;
            Level = 9990;
        }

        /// <summary>
        /// 检查是否有缓存 如果有缓存标志 直接输出缓存 并且下面的代码将不再运行
        /// </summary>
        /// <returns></returns>
        public override bool IsValidate()
        {
            if (base.IsValidate())
            {
                //检查当前是否有缓存标志
                if (CurrentHttpRequest.Context.Request.Headers.AllKeys.Contains(CacheName))
                {
                    DateTime dt = Convert.ToDateTime(CurrentHttpRequest.Context.Request.Headers[CacheName]);
                    //判断是否在缓存时间内
                    if ((DateTime.Now - dt).TotalSeconds < _seconds)
                    {
                        //在缓存时间里面  这里会输出304的头部 并且停止页面的输出
                        //通知客户端以缓存输出
                        CurrentHttpRequest.Context.Response.StatusCode = 304;
                        CurrentHttpRequest.Context.Response.End();
                        return true;
                    }
                }

                // 设置最近修改的响应头Last-Modified，客户端将会发送If-Modified-Since到服务器端
                // 配合上面代码实现缓存
                CurrentHttpRequest.Context.Response.Cache.SetLastModified(DateTime.Now);

                return true;
            }
            return false;
        }
    }
}
