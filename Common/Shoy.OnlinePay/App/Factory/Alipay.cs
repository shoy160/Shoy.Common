using Shoy.OnlinePay.App.Domain;
using Shoy.OnlinePay.App.Utils;
using Shoy.Utility;
using Shoy.Utility.Helper;
using System.Collections.Generic;

namespace Shoy.OnlinePay.App.Factory
{
    /// <summary> 支付宝 </summary>
    public class Alipay : DPay
    {
        public Alipay()
        {
            Config = Get(PaidType.Alipay);
        }

        //public override DApiResult<string> Pay(long orderNo, decimal price, string subject)
        //{
        //    var dict = Config.BaseParams("alipay.trade.pay");
        //    //业务参数
        //    var item = new Dictionary<string, object>
        //    {
        //        {"out_trade_no", orderNo},
        //        {"product_code", "QUICK_MSECURITY_PAY"},
        //        {"auth_code", "28763443825664394"},
        //        {"subject", subject},
        //        {"total_amount", price}
        //    };
        //    dict.Add("biz_content", JsonHelper.ToJson(item));
        //    dict.Add("sign", dict.RsaSign(Config.PrivateKey, Config.Charset));

        //    var url = $"{Config.Gateway}?{dict.ParamsUrl()}";
        //    Logger.Info(JsonHelper.ToJson(dict, indented: true));
        //    using (var http = new HttpHelper(url, Encoding.GetEncoding(Config.Charset)))
        //    {
        //        var html = http.GetHtml();
        //        Logger.Info(html);
        //        var dto = JsonHelper.Json<ReturnAlipayDto>(html);
        //        if (dto?.alipay_trade_pay_response == null)
        //            return DApiResult.Error<string>("支付接口异常");
        //        var result = dto.alipay_trade_pay_response;
        //        if (result.code != "10000")
        //            return DApiResult.Error<string>($"{result.msg}:{result.sub_msg}");
        //        return DApiResult.Succ(string.Empty);
        //    }
        //}

        public override DResult<VerifyDto> Verify()
        {
            var paramDict = OnlinePayHelper.GetParams();
            Logger.Info(JsonHelper.ToJson(paramDict, indented: true));
            var signVerified = AlipaySignature.RsaCheck(paramDict, Config.PublicKey, Config.Charset);
            if (!signVerified) return DResult.Error<VerifyDto>("验证签名失败");
            if (paramDict.GetValue<string>("app_id") != Config.AppId)
                return DResult.Error<VerifyDto>("AppId异常");
            var dto = new VerifyDto
            {
                Id = paramDict.GetValue<string>("out_trade_no"),
                TradeNo = paramDict.GetValue<string>("trade_no"),
                TradeStatus = paramDict.GetValue<string>("trade_status"),
                Amount = paramDict.GetValue<decimal>("total_amount"),
                BuyerId = paramDict.GetValue<string>("buyer_id"),
                BuyerAccount = paramDict.GetValue<string>("buyer_logon_id")
            };
            return DResult.Succ(dto);
        }

        public override DResult<Dictionary<string, string>> Request(string tradeNo, decimal price, string subject)
        {
            var dict = Config.BaseParams("alipay.trade.app.pay");
            //业务参数
            var item = new Dictionary<string, string>
            {
                {"subject", subject},
                {"out_trade_no", tradeNo},
                {"total_amount", price.ToString("f2")},
                {"product_code", "QUICK_MSECURITY_PAY"}
            };
            dict.Add("biz_content", JsonHelper.ToJson(item));
            dict.Add("sign", dict.RsaSign(Config.PrivateKey, Config.Charset));
            return DResult.Succ(dict);
        }
    }
}
