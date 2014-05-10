using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shoy.OtherPlatform.Entity;
using System.Web;

namespace Shoy.OtherPlatform
{
    public  class PlatformBLL
    {
        
        #region 腾讯业务
        public string GetLoginUrl(PlatformType p, string callBackUrl)//现在没有其他权限 底层留了参数这里先暂时不要。params string[] scopes到时候不会影响没有传参的调用
        {
           var f = PlatformFactory.GetInstance(p);
            return f.CreateLoginUrl(callBackUrl);
        }
        public TencentUserInfo GetTencentUserInfo(HttpContext h,string callBackUrl)
        {
            var f = PlatformFactory.GetInstance(PlatformType.Tencent);//后期把这个东西放到缓存里面
            return f.GetUserInfo(h, callBackUrl) as TencentUserInfo;
        }
        #endregion

        #region 新浪业务
        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="h"></param>
        /// <returns></returns>
        public SinaWeiboUserInfo GetSinaUserInfo(HttpContext h, string callBackUrl)
        {
            if (string.IsNullOrEmpty(h.Request.QueryString["code"]))
            {
                return null;
            }
            var f = PlatformFactory.GetInstance(PlatformType.SinaWeibo);//后期把这个东西放到缓存里面
            return f.GetUserInfo(h,callBackUrl) as SinaWeiboUserInfo;
        }
        #endregion

        #region 阿里业务
        public ali_notify_info GetAliPayUserInfo()
        {
            string backurl="";
            var f = PlatformFactory.GetInstance(PlatformType.Alipay);//后期把这个东西放到缓存里面
            return f.GetUserInfo(null, backurl) as ali_notify_info;
        }
        #endregion
    }
}
