
using System.ComponentModel;

namespace Shoy.ThirdPlatform.Entity.Config
{
    public enum PlatformType
    {
        /// <summary> QQ </summary>
        [Description("腾讯QQ")]
        Tencent = 0,

        /// <summary> 微博 </summary>
        [Description("微博")]
        Weibo = 1,
        /// <summary> 腾讯微博 </summary>
        [Description("腾讯微博")]
        TencentWeibo = 2,
        /// <summary> 微信 </summary>
        [Description("微信")]
        Weixin = 3,
        /// <summary> 支付宝 </summary>
        [Description("支付宝")]
        Alipay = 4
    }
}
