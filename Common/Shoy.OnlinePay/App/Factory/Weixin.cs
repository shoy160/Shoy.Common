using Shoy.OnlinePay.App.Domain;
using Shoy.Utility;
using Shoy.Utility.Extend;
using Shoy.Utility.Helper;
using Shoy.Utility.Timing;
using System.Collections.Generic;
using System.Text;

namespace Shoy.OnlinePay.App.Factory
{
    /// <summary> 微信 </summary>
    public class Weixin : DPay
    {
        public Weixin()
        {
            Config = Get(PaidType.Weixin);
        }
        private DResult<string> GetPrepayId(string tradeNo, decimal price, string subject)
        {
            var encoding = Encoding.GetEncoding(Config.Charset);
            var dict = Config.BaseParams();
            dict.Add("out_trade_no ", tradeNo);
            dict.Add("total_fee", price.ToString("f2"));
            dict.Add("body", subject);
            dict.Add("sign", dict.Md5Sign(Config.PrivateKey));
            Logger.Info(JsonHelper.ToJson(dict, indented: true));
            using (var http = new HttpHelper(Config.Gateway, "POST", encoding, dict.ParamsXml()))
            {
                http.SetContentType("text/xml");
                var html = http.GetHtml();
                Logger.Info(html);
                html = html.As<IRegex>().Replace("\n", string.Empty);
                var dto = XmlHelper.XmlDeserialize<ReturnWeixinDto>(html, encoding);
                if (dto == null || dto.return_code != "SUCCESS")
                    return DResult.Error<string>(dto?.return_msg ?? "支付接口异常");
                return DResult.Succ(dto.prepay_id);
            }
        }

        public override DResult<VerifyDto> Verify()
        {
            var paramDict = OnlinePayHelper.GetParams();
            Logger.Info(JsonHelper.ToJson(paramDict, indented: true));
            var sign = paramDict.GetValue<string>("sign");
            var checkSign = paramDict.Md5Sign(Config.PrivateKey);
            if (sign != checkSign)
                return DResult.Error<VerifyDto>("验证签名失败");
            if (Config.AppId != paramDict.GetValue<string>("appid"))
                return DResult.Error<VerifyDto>("AppId异常");
            var dto = new VerifyDto
            {
                Id = paramDict.GetValue<string>("out_trade_no"),
                TradeNo = paramDict.GetValue<string>("transaction_id"),
                TradeStatus = paramDict.GetValue<string>("result_code"),
                Amount = paramDict.GetValue<decimal>("total_fee"),
                BuyerId = paramDict.GetValue<string>("openid")
            };
            return DResult.Succ(dto);
        }

        public override DResult<Dictionary<string, string>> Request(string tradeNo, decimal price, string subject)
        {
            var result = GetPrepayId(tradeNo, price, subject);
            if (!result.Status)
                return DResult.Error<Dictionary<string, string>>(result.Message);
            var prepayId = result.Data;
            var dict = new Dictionary<string, string>
            {
                {"appId", Config.AppId},
                {"partnerId ", Config.PartnerId},
                {"prepayId", prepayId},
                {"packageValue", "Sign=WXPay"},
                {"nonceStr", IdHelper.Instance.Guid32},
                {"timeStamp", Clock.Now.ToLong().ToString()}
            };
            dict.Add("sign", dict.Md5Sign(Config.PrivateKey));
            return DResult.Succ(dict);
        }
    }
}
