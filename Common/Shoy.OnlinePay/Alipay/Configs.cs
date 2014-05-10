using Shoy.OnlinePay.Common;
namespace Shoy.OnlinePay.Alipay
{
    public static class Configs
    {
        /// <summary>
        /// 支付接口
        /// </summary>
        //public const string Getway = "https://www.alipay.com/cooperate/gateway.do?";
        public const string Getway = "https://mapi.alipay.com/gateway.do?";//2013-03-07支付宝新网关;

        //"https://www.alipay.com/cooperate/gateway.do?service=notify_verify"; 旧网关
        /// <summary>
        /// 参数验证端口
        /// </summary>
        public const string VerifyUrl = "https://mapi.alipay.com/gateway.do?service=notify_verify";//2013-03-07支付宝新网关

        /// <summary>
        /// 服务名称
        /// </summary>
        public const string Service = "create_direct_pay_by_user";

        public static string GetSellerEmail()
        {
            var item = Utils.GetPartnerInfos(PayType.Alipay);
            return (item == null || string.IsNullOrEmpty(item.SellerEmail)) ? SellerEmail : item.SellerEmail;
        }

        /// <summary>
        /// 商家签约帐号
        /// </summary>
        private const string SellerEmail = "384843323@qq.com";

        public static string GetPartnerId()
        {
            var item = Utils.GetPartnerInfos(PayType.Alipay);
            return item == null ? "" : item.PartnerId;
        }

        public static string GetKey()
        {
            var item = Utils.GetPartnerInfos(PayType.Alipay);
            return item == null ? "" : item.Key;
        }

        #region 百货购支付宝

        ///// <summary>
        ///// 安全校验码
        ///// </summary>
        //public const string Key = "37wuz4e2jv7ic7x8ytm8pej7ilyb96k0";

        ///// <summary>
        ///// 合作商户Id
        ///// </summary>
        //public const string PartnerId = "2088102957310471";

        #endregion

        #region 惠惠网支付宝

        ///// <summary>
        ///// 安全校验码
        ///// </summary>
        //public const string Key = "2ulg6qnxhznvkzheoqym2rbt16ohiqqp";

        ///// <summary>
        ///// 合作商户Id
        ///// </summary>
        //public const string PartnerId = "2088901725096489";

        #endregion

        public const string SignType = "MD5";

        public const string Charset = "utf-8";
    }
}
