using Shoy.OnlinePay.Common;

namespace Shoy.OnlinePay.MAlipay
{
    public class Config
    {

        public static string PublicKey
        {
            get
            {
                var item = Utils.GetPartnerInfos(PayType.MAlipay);
                return item == null ? "" : item.Key;
            }
        }
        
        public const string TRADEFINISHED = "TRADE_FINISHED";
        public const string TRADESUCCESS = "TRADE_SUCCESS";
    }
}
