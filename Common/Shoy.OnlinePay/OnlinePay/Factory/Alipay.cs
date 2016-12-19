using DayEasy.God.Contracts.Dtos;
using DayEasy.God.Services.OnlinePay.Domain;
using DayEasy.Utility.Helper;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DayEasy.God.Services.OnlinePay.Factory
{
    /// <summary> 支付宝 </summary>
    public class Alipay : DPay
    {
        public Alipay()
        {
            Config = Get(PaidType.Alipay);
        }

        public override DApiResult<string> Pay(long orderNo, decimal price, string subject)
        {
            var dict = Config.BaseParams("alipay.trade.pay");
            //业务参数
            var item = new Dictionary<string, object>
            {
                {"out_trade_no", orderNo},
                {"product_code", "QUICK_MSECURITY_PAY"},
                {"auth_code", "28763443825664394"},
                {"subject", subject},
                {"total_amount", price}
            };
            dict.Add("biz_content", JsonHelper.ToJson(item));
            dict.Add("sign", dict.RsaSign(Config.PrivateKey, Config.Charset));

            var url = $"{Config.Gateway}?{dict.ParamsUrl()}";
            Logger.Info(JsonHelper.ToJson(dict, indented: true));
            using (var http = new HttpHelper(url, Encoding.GetEncoding(Config.Charset)))
            {
                var html = http.GetHtml();
                Logger.Info(html);
                var dto = JsonHelper.Json<ReturnAlipayDto>(html);
                if (dto?.alipay_trade_pay_response == null)
                    return DApiResult.Error<string>("支付接口异常");
                var result = dto.alipay_trade_pay_response;
                if (result.code != "10000")
                    return DApiResult.Error<string>($"{result.msg}:{result.sub_msg}");
                return DApiResult.Succ(string.Empty);
            }
        }

        public override DApiResult<VerifyDto> Verify()
        {
            var input = System.Web.HttpContext.Current.Request.InputStream;
            input.Seek(0, SeekOrigin.Begin);
            using (var stream = new StreamReader(input))
            {
                var body = stream.ReadToEnd();
                Logger.Info(body);
            }
            return DApiResult.Error<VerifyDto>("未实现");
        }

        public override IDictionary<string, object> Request(string prepayId = null)
        {
            throw new System.NotImplementedException();
        }
    }
}
