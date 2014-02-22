using System;
using System.Text;
using System.Web;
using System.Linq;
using Shoy.OnlinePay.Common;

namespace Shoy.OnlinePay.Alipay
{
    public class Base:OnlineUtils
    {
        public override string CreateUrl(ParameterInfo alipay)
        {
            var info = alipay as AlipayParasInfo;
            if (info == null) return "";
            //构造数组；
            //以下数组即是参与加密的参数，若参数的值不允许为空，若该参数为空，则不要成为该数组的元素
            string[] paras = {
                                 "service=" + Configs.Service,
                                 "partner=" + Configs.GetPartnerId(),
                                 "seller_email=" + Configs.GetSellerEmail(),
                                 "out_trade_no=" + info.OrderNum,
                                 "subject=" + info.ProName + "[" + info.Amount.ToString("0.00") + "]元",
                                 "body=" + info.ProName,
                                 "total_fee=" + info.Amount.ToString("0.00"),
                                 "show_url=" + info.ShowUrl,
                                 "payment_type=1",
                                 "notify_url=" + info.NotifyUrl,
                                 "return_url=" + info.ReturnUrl,
                                 "_input_charset=utf-8",
                                 "buyer_email=" + info.Account
                             };

            var sortedstr = Digest.BubbleSort(paras);

            //构造待md5摘要字符串
            var prestr = new StringBuilder();

            //以下是GET方式传递参数
            //构造支付Url；
            var parameter = new StringBuilder();

            parameter.Append(Configs.Getway);

            for (var i = 0; i < sortedstr.Length; i++)
            {
                if (i == sortedstr.Length - 1)
                {
                    prestr.Append(sortedstr[i]);

                }
                else
                {
                    prestr.Append(sortedstr[i] + "&");
                }
                var para = sortedstr[i].Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                //UTF-8格式的编码转换
                parameter.Append(para[0] + "=" + HttpUtility.UrlEncode(para[1], Encoding.UTF8) + "&");
            }

            prestr.Append(Configs.GetKey());

            //签名
            var sign = Digest.GetMd5(prestr.ToString(), "utf-8");


            parameter.Append("sign=" + sign + "&sign_type=" + Configs.SignType);

            //返回支付Url；
            return parameter.ToString();
        }

        public override BaseResult VerifyCallBack(HttpRequest request)
        {
            var result = new AlipayResult();

            var coll = (string.IsNullOrEmpty(request.Form["out_trade_no"]) ? request.QueryString : request.Form);

            result.TradeNum = coll["out_trade_no"];
            result.Amount = Convert.ToDecimal(coll["total_fee"]);
            result.BuyerEmail = coll["buyer_email"];

            //支付宝ATN验证
            var verifyUrl = string.Format(Configs.VerifyUrl + "&partner={0}&notify_id={1}", Configs.GetPartnerId(),
                                          coll["notify_id"]);
            var responseText = Utils.GetHttp(verifyUrl, 120*1000);
            if (responseText != "true")
            {
                result.State = false;
                result.ErrMsg = "支付宝ATN验证无效！";
                return result;
            }

            //签名验证 Start...

            int i;

            // Get names of all forms into a string array.
            String[] requestarr = coll.AllKeys;

            //进行排序；
            string[] sortedstr = Digest.BubbleSort(requestarr);

            //构造待md5摘要字符串 ；

            var prestr = new StringBuilder();

            for (i = 0; i < sortedstr.Length; i++)
            {
                var noIn = new[] {"subdomain", "urlpath", "param", "sign", "sign_type"};
                if (!string.IsNullOrEmpty(coll[sortedstr[i]]) && !noIn.Contains(sortedstr[i]))
                {
                    if (i == sortedstr.Length - 1)
                    {
                        prestr.Append(sortedstr[i] + "=" + coll[sortedstr[i]]);
                    }
                    else
                    {
                        prestr.Append(sortedstr[i] + "=" + coll[sortedstr[i]] + "&");
                    }
                }
            }
            prestr.Append(Configs.GetKey());

            //生成Md5摘要；
            var ourSign = Digest.GetMd5(prestr.ToString(), Configs.Charset);
            //*******加密签名程序结束*******

            var sign = coll["sign"];

            if (sign != ourSign)
            {
                result.State = false;
                result.ErrMsg = "签名验证失败-" + sign + " | " + ourSign;
            }
            var state = new[] {"TRADE_FINISHED", "TRADE_SUCCESS"};
            result.TradeStatus = coll["trade_status"];
            if (!state.Contains(result.TradeStatus))
            {
                result.State = false;
                result.ErrMsg = "支付状态失败！";
            }
            else
                result.State = true;
            return result;
        }
    }
}
