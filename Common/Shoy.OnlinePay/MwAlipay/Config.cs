using Shoy.OnlinePay.Common;

namespace Shoy.OnlinePay.MwAlipay
{
    public class Config
    {
        private const string SellerEmail = "1718668746@qq.com";

        public const string SignType = "MD5";

        public const string Charset = "utf-8";

        public const string Format = "xml";

        public const string Version = "2.0";

        public const string ServiceTrade = "alipay.wap.trade.create.direct";
        
        public const string ServiceAuth = "alipay.wap.auth.authAndExecute";   

        public const string Getway = "http://wappaygw.alipay.com/service/rest.htm?"; //支付网关

        public const string TRADEFINISHED = "TRADE_FINISHED";
        public const string TRADESUCCESS = "TRADE_SUCCESS";

        public static string GetSellerEmail()
        {
            var item = Utils.GetPartnerInfos(PayType.MwAlipay);
            return (item == null || string.IsNullOrEmpty(item.SellerEmail)) ? SellerEmail : item.SellerEmail;
        }

        public static string GetPartnerId()
        {
            var item = Utils.GetPartnerInfos(PayType.MwAlipay);
            return item == null ? "" : item.PartnerId;
        }

        public static string GetKey()
        {
            var item = Utils.GetPartnerInfos(PayType.MwAlipay);
            return item == null ? "" : item.Key;
        }
    }
}
