using System;
using System.Linq;
using Shoy.OnlinePay.Common;
using System.Text;
using System.Web;
using System.Collections.Specialized;

namespace Shoy.OnlinePay.YeePay
{
    public class Base:OnlineUtils
    {
        public override string CreateUrl(ParameterInfo yeepay)
        {
            var info = yeepay as YeePayParasInfo;
            if (info == null) return "";
            //签名
            var sb = "Buy";
            sb += Configs.GetPartnerId();
            sb += info.OrderNum;
            sb += info.Amount.ToString("0.00");
            sb += info.Cur;
            sb += info.ProName;
            sb += info.ProCate;
            sb += info.ProDesc;
            sb += info.ReturnUrl;
            sb += info.Saf;
            sb += info.ExtentInfo;
            sb += info.FrpId;
            sb += (info.NeedResponse ? "1" : "0");
            var hmac = Digest.HmacSign(sb, Configs.GetKey());

            var url = Configs.RequestPayUrl;
            url += string.Format(Configs.Paras, Configs.GetPartnerId(),
                                 HttpUtility.UrlEncode(info.OrderNum, Configs.DefCoding), info.Amount.ToString("0.00"), info.Cur,
                                 HttpUtility.UrlEncode(info.ProName, Configs.DefCoding),
                                 HttpUtility.UrlEncode(info.ProCate, Configs.DefCoding),
                                 HttpUtility.UrlEncode(info.ProDesc, Configs.DefCoding),
                                 HttpUtility.UrlEncode(info.ReturnUrl, Configs.DefCoding),
                                 info.Saf, HttpUtility.UrlEncode(info.ExtentInfo, Configs.DefCoding), info.FrpId,
                                 (info.NeedResponse ? "1" : "0"), hmac);
            return url;
        }

        public override BaseResult VerifyCallBack(HttpRequest request)
        {
            var result = new YeepayResult();
            //var coll = (string.IsNullOrEmpty(request.Form["r6_Order"]) ? request.QueryString : request.Form);
            var list = Decode(request);
            result.TradeNum = list["r6_Order"];
            result.Amount = Convert.ToDecimal(list["r3_Amt"]);
            result.NeedResponse = (list["r9_BType"] == "2");
            result.Code = list["r1_Code"];

            result.PayDate = Utils.StrToDate(list["rp_PayDate"], DateTime.Now);
            result.TrxId = list["r2_TrxId"];
            if (result.Code != "1")
            {
                result.State = false;
                result.ErrMsg = "支付状态失败！";
                return result;
            }
            var qs = new[]
                         {
                             "p1_MerId", "r0_Cmd", "r1_Code", "r2_TrxId", "r3_Amt", "r4_Cur", "r5_Pid", "r6_Order",
                             "r7_Uid", "r8_MP", "r9_BType"
                         };
            var nhmac = qs.Aggregate("", (current, t) => current + list[t]);
            var ourSign = Digest.HmacSign(nhmac, Configs.GetKey());
            var sign = list["hmac"];
            if (sign != ourSign)
            {
                result.State = false;
                result.ErrMsg = string.Format("签名验证失败--{0}|{1}", sign, ourSign);
            }
            else
                result.State = true;
            return result;
        }

        private static NameValueCollection Decode(HttpRequest request)
        {
            var result = new NameValueCollection();
            try
            {
                var qStr = request.RawUrl.Split('?')[1];
                var list = qStr.Split('&');
                if (list.Length == 0)
                    return result;
                foreach (var s in list)
                {
                    var item = s.Split('=');
                    if (item.Length == 2)
                    {
                        result.Add(item[0], HttpUtility.UrlDecode(item[1], Configs.DefCoding));
                    }
                }
            }
            catch{}
            return result;
        }
    }
}