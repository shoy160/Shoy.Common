using System.ComponentModel;
namespace Shoy.OnlinePay
{
    public enum PayType
    {
        [Description("支付宝")]
        Alipay = 1,
        [Description("易宝支付")]
        YeePay = 2,
        [Description("手机支付宝")]
        MAlipay = 3,
        [Description("银联支付")]
        UnionPay = 4,
        [Description("手机网站支付宝")]
        MwAlipay = 5
    }
}
