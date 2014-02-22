using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using Shoy.OnlinePay.Common;
using Shoy.Utility;

namespace Shoy.OnlinePay.MwAlipay
{
    public class Base : OnlineUtils
    {
        public override string CreateUrl(ParameterInfo mwAlipay)
        {
            var info = mwAlipay as MwAlipayInfo;
            if (info == null)
                return "";
            //请求业务参数详细
            string reqDataToken = "<direct_trade_create_req><notify_url>" + info.NotifyUrl + "</notify_url><call_back_url>" + info.ReturnUrl + "</call_back_url><seller_account_name>" + Config.GetSellerEmail() + "</seller_account_name><out_trade_no>" + info.OrderNum + "</out_trade_no><subject>" + HttpUtility.HtmlEncode(info.ProName) + "</subject><total_fee>" + info.Amount.ToString("0.00") + "</total_fee><merchant_url>" + info.ReturnUrl + "</merchant_url></direct_trade_create_req>";

            var sParaTempToken = new Dictionary<string, string>
                                     {
                                         {"partner", Config.GetPartnerId()},
                                         {"_input_charset", Config.Charset},
                                         {"sec_id", Config.SignType.ToUpper()},
                                         {"service", Config.ServiceTrade},
                                         {"format", Config.Format},
                                         {"v", Config.Version},
                                         {"req_id", info.OrderNum},
                                         {"req_data", reqDataToken}
                                     };
            var encoding = Encoding.GetEncoding(Config.Charset);
            sParaTempToken = Common.Utils.BuildParas(sParaTempToken, Config.GetKey(), Config.Charset);
            var paras = Common.Utils.CreateLinkString(sParaTempToken,encoding);
            var token = "";
            const string gateWay = Config.Getway + "_input_charset=" + Config.Charset;
            using (var http = new HttpHelper(gateWay, "POST", encoding, paras))
            {
                token = http.GetHtml();
            }
            //解析token
            var requestToken = Common.Utils.GetRequestToken(token,encoding);
            string reqData = "<auth_and_execute_req><request_token>" + requestToken + "</request_token></auth_and_execute_req>";
            var sParaTemp = new Dictionary<string, string>
                                {
                                    {"partner", Config.GetPartnerId()},
                                    {"_input_charset", Config.Charset.ToLower()},
                                    {"sec_id", Config.SignType.ToUpper()},
                                    {"service", Config.ServiceAuth},
                                    {"format", Config.Format},
                                    {"v", Config.Version},
                                    {"req_data", reqData}
                                };
            sParaTemp = Common.Utils.BuildParas(sParaTemp, Config.GetKey(), Config.Charset);

            return Common.Utils.BuildRequest(Config.Getway, sParaTemp);
        }

        public override BaseResult VerifyCallBack(HttpRequest request)
        {
            var mwItem = new MwAlipayResult() {State = false};

            var sPara = GetRequestGet(request);
            var isNotify = (request.HttpMethod.ToLower() == "post");

            var sign = sPara["sign"];

            if (sPara.Count > 0)
            {
                var mySign = (isNotify
                                     ? Common.Utils.BuildNotifyParas(sPara, Config.GetKey(), Config.Charset)["sign"]
                                     : Common.Utils.BuildParas(sPara, Config.GetKey(), Config.Charset)["sign"]);
                if (mySign == sign)
                {
                    if (!isNotify)
                    {
                        mwItem.TradeNum = sPara["out_trade_no"];
                        mwItem.trade_no = sPara["trade_no"];
                        mwItem.State = sPara["result"] == "success";
                    }
                    else
                    {
                        var xml = new Function.XmlDoc(sPara["notify_data"]);
                        //验证是否为支付宝发起的请求
                        string notify_id = xml.GetNode("/notify/notify_id");
                        if(GetResponseTxt(notify_id) != "true")
                        {
                            mwItem.State = false;
                            mwItem.ErrMsg = "非支付宝发起请求";
                            return mwItem;
                        }

                        string tradeStatus = xml.GetNode("notify/trade_status");
                        if (tradeStatus != Config.TRADEFINISHED && tradeStatus != Config.TRADESUCCESS)
                        {
                            mwItem.State = false;
                            mwItem.ErrMsg = "交易状态：" + tradeStatus;
                            return mwItem;
                        }
                        mwItem.Amount = Convert.ToDecimal(xml.GetNode("notify/total_fee"));
                        mwItem.TradeNum = xml.GetNode("notify/out_trade_no");
                        mwItem.trade_no = xml.GetNode("notify/trade_no");
                        mwItem.State = true;
                    }
                }
            }

            return mwItem;
        }

        /// <summary>
        /// 获取支付宝GET过来通知消息
        /// </summary>
        /// <returns>request回来的信息组成的数组</returns>
        public Dictionary<string, string> GetRequestGet(HttpRequest request)
        {
            var coll = (request.HttpMethod.ToLower() == "post" ? request.Form : request.QueryString);

            String[] requestItem = coll.AllKeys;

            return requestItem.ToDictionary(t => t, t => HttpUtility.HtmlDecode(coll[t]));
        }

        /// <summary>
        /// 获取是否是支付宝服务器发来的请求的验证结果
        /// </summary>
        /// <param name="notify_id">通知验证ID</param>
        /// <returns>验证结果</returns>
        public string GetResponseTxt(string notify_id)
        {
            string veryfy_url = "https://mapi.alipay.com/gateway.do?service=notify_verify&partner=" + Config.GetPartnerId() + "&notify_id=" + notify_id;

            //获取远程服务器ATN结果，验证是否是支付宝服务器发来的请求

            string strResult;
            try
            {
                var myReq = (HttpWebRequest)HttpWebRequest.Create(veryfy_url);
                myReq.Timeout = 120000;
                var HttpWResp = (HttpWebResponse)myReq.GetResponse();
                var myStream = HttpWResp.GetResponseStream();
                var sr = new StreamReader(myStream, Encoding.Default);
                var strBuilder = new StringBuilder();
                while (-1 != sr.Peek())
                {
                    strBuilder.Append(sr.ReadLine());
                }
                strResult = strBuilder.ToString();
            }
            catch (Exception exp)
            {
                strResult = "错误：" + exp.Message;
            }
            return strResult;
        }
    }
}
