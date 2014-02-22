using System.Text;
using Shoy.OnlinePay.Common;

namespace Shoy.OnlinePay.UnionPay
{
    public class Configs
    {
        //public const string RequestPayUrl = "https://unionpaysecure.com/api/Pay.action";
        public const string RequestPayUrl = "http://58.246.226.99/UpopWeb/api/Pay.action";

        public static Encoding DefEncoding = Encoding.UTF8;

        public const string Version = "1.0.0";
        public const string MemberName = "百货购";
        public static string PartnerId
        {
            get
            {
                var item = Utils.GetPartnerInfos(PayType.UnionPay);
                return item == null ? "" : item.PartnerId;
            }
        }

        public static string PartnerKey
        {
            get
            {
                var item = Utils.GetPartnerInfos(PayType.UnionPay);
                return item == null ? "" : item.Key;
            }
        }

        public static string[] ParaNames = new[]
                                               {
                                                   "version", "charset", "transType", "origQid", "merId", "merAbbr",
                                                   "acqCode", "merCode", "commodityUrl", "commodityName",
                                                   "commodityUnitPrice", "commodityQuantity", "commodityDiscount",
                                                   "transferFee", "orderNumber", "orderAmount", "orderCurrency",
                                                   "orderTime", "customerIp", "customerName", "defaultPayType",
                                                   "defaultBankNumber", "transTimeout", "frontEndUrl", "backEndUrl",
                                                   "merReserved"
                                               };
    }
}
