using DayEasy.Core;
using DayEasy.God.Contracts.Dtos;
using DayEasy.God.Services.OnlinePay.Domain;
using DayEasy.Utility.Extend;
using DayEasy.Utility.Helper;
using DayEasy.Utility.Timing;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DayEasy.God.Services.OnlinePay.Factory
{
    /// <summary> 微信 </summary>
    public class Weixin : DPay
    {
        public Weixin()
        {
            Config = Get(PaidType.Weixin);
        }
        public override DApiResult<string> Pay(long orderNo, decimal price, string subject)
        {
            var encoding = Encoding.GetEncoding(Config.Charset);
            var dict = Config.BaseParams();
            dict.Add("out_trade_no ", orderNo);
            dict.Add("total_fee", price);
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
                    return DApiResult.Error<string>(dto?.return_msg ?? "支付接口异常");
                return DApiResult.Succ(dto.prepay_id);
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
            var dict = new Dictionary<string, object>
            {
                {"appId", Config.AppId},
                {"partnerId ", Config.AppId},
                {"prepayId", prepayId},
                {"packageValue", "Sign=WXPay"},
                {"nonceStr", IdHelper.Instance.Guid32},
                {"timeStamp", Clock.Now.ToLong()}
            };
            dict.Add("sign", dict.Md5Sign(Config.PrivateKey));
            return dict;
        }
    }
}
