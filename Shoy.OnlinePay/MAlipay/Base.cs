using System;
using System.Web;
using Shoy.OnlinePay.Common;

namespace Shoy.OnlinePay.MAlipay
{
    public class Base:OnlineUtils
    {
        public override string CreateUrl(ParameterInfo info)
        {
            return "";
        }

        public override BaseResult VerifyCallBack(HttpRequest request)
        {
            var result = new MAlipayResult();

            var coll = (request.HttpMethod.ToLower() == "post" ? request.Form : request.QueryString);
            string notifyData = coll["notify_data"],
                   sign = coll["sign"];
            if (string.IsNullOrEmpty(notifyData) || string.IsNullOrEmpty(sign))
            {
                result.State = false;
                result.ErrMsg = "notify_data或sign参数为空";
                return result;
            }
            var verify = Function.Verify("notify_data=" + notifyData, sign, Config.PublicKey);
            if (!verify)
            {
                result.State = false;
                result.ErrMsg = "签名验证失败";
                return result;
            }
            var xml = new Function.XmlDoc(notifyData);
            string tradeStatus = xml.GetNode("notify/trade_status");
            if (tradeStatus != Config.TRADEFINISHED && tradeStatus != Config.TRADESUCCESS)
            {
                result.State = false;
                result.ErrMsg = "交易状态：" + tradeStatus;
                return result;
            }
            result.State = true;
            result.Amount = Convert.ToDecimal(xml.GetNode("notify/total_fee"));
            result.TradeNum = xml.GetNode("notify/out_trade_no");
            result.TradeNo = xml.GetNode("notify/trade_no");
            result.TradeStatus = tradeStatus;
            result.BuyerEmail = xml.GetNode("notify/buyer_email");
            return result;
        }
    }
}
