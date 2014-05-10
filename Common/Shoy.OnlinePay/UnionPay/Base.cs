using System;
using System.Collections.Generic;
using System.Web;
using Shoy.OnlinePay.Common;
using System.Text;
using com.unionpay.upop.sdk;

namespace Shoy.OnlinePay.UnionPay
{
    public class Base:OnlineUtils
    {
        public override string CreateUrl(ParameterInfo unionPay)
        {
            var info = unionPay as UnionPayParasInfo;
            if (info == null)
                return "";
            UPOPSrv.LoadConf(HttpContext.Current.Server.MapPath("~/App_Data/xml/unionPay.config"));

            var paras = new Dictionary<string, string>
                            {
                                {"transType", "01"},
                                {"commodityUrl", Uri.EscapeUriString(info.ProductUrl)},
                                {"commodityName", info.ProductName},
                                {"commodityUnitPrice", info.UnitPrice.ToString()},
                                {"orderNumber", info.OrderNum},
                                {"orderAmount", (Math.Round(info.Amount, 2)*100).ToString("F0")},
                                {"orderCurrency", info.Cur},
                                {"orderTime", DateTime.Now.ToString("yyyyMMddHHmmss")},
                                {"customerIp", Utils.GetRealIp()},
                                {"frontEndUrl", info.ReturnUrl},
                                {"backEndUrl", info.NotifyUrl}
                            };
            var srv = new FrontPaySrv(paras);
            return srv.CreateHtml();
        }

        public override BaseResult VerifyCallBack(HttpRequest request)
        {
            var result = new UnionPayResult();
            var coll = request.HttpMethod.ToLower() == "post"
                           ? request.Form
                           : request.QueryString;
            UPOPSrv.LoadConf(HttpContext.Current.Server.MapPath("~/App_Data/xml/unionPay.config"));
            var srv = new SrvResponse(Util.NameValueCollection2StrDict(coll));
            if (srv.ResponseCode != SrvResponse.RESP_SUCCESS)
            {
                result.State = false;
                result.ErrMsg = "返回状态异常:" + srv.ResponseCode;
                return result;
            }

            result.State = true;
            //金额，银联是以分为单位，故要除以100
            result.Amount = Convert.ToInt32(srv.Fields["orderAmount"])/100M;
            result.TradeNum = srv.Fields["orderNumber"];
            return result;
        }
    }
}
