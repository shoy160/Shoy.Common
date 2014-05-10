using System.Text;
using Shoy.OnlinePay.Common;
namespace Shoy.OnlinePay.YeePay
{
    public static class Configs
    {
        /// <summary>
        /// 支付请求地址
        /// </summary>
        public const string RequestPayUrl = "https://www.yeepay.com/app-merchant-proxy/node";

        /// <summary>
        /// 退款请求地址
        /// </summary>
        public const string RequestRefundUrl = "https://www.yeepay.com/app-merchant-proxy/command";

        public static string GetPartnerId()
        {
            var item = Utils.GetPartnerInfos(PayType.YeePay);
            return item == null ? "" : item.PartnerId;
        }

        public static string GetKey()
        {
            var item = Utils.GetPartnerInfos(PayType.YeePay);
            return item == null ? "" : item.Key;
        }

        public const string Paras =
            "?p0_Cmd=Buy&p1_MerId={0}&p2_Order={1}&p3_Amt={2}&p4_Cur={3}&p5_Pid={4}&p6_Pcat={5}&p7_Pdesc={6}&p8_Url={7}&p9_SAF={8}&pa_MP={9}&pd_FrpId={10}&pr_NeedResponse={11}&hmac={12}";

        public static readonly Encoding DefCoding = Encoding.GetEncoding("gb2312");
    }
}
