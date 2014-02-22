using System.ComponentModel;

namespace Shoy.OtherPlatform
{
    public enum PlatformType
    {
        [Description("支付宝")]
        Alipay = 1,
        [Description("腾讯QQ")]
        Tencent = 2,
        [Description("新浪微博")]
        SinaWeibo = 3,
        [Description("腾讯微博")]
        TenWeibo=4
    }
}
