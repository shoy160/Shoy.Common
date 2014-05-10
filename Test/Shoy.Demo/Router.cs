using Shoy.AjaxHelper;
using System.Collections.Specialized;
using System.Web;
using System.Text.RegularExpressions;
using Shoy.AjaxHelper.Model;

namespace Shoy.Demo
{
    public class Router:IAjax
    {
        [AjaxAction(RequestType.Get)]
        public string Test()
        {
            return "hello!";
        }

        [AjaxAction(RequestType.All)]
        public object Verify(string partner, string method, string sign)
        {
            NameValueCollection qs = (HttpContext.Current.Request.HttpMethod.ToLower() == "post"
                                          ? HttpContext.Current.Request.Form
                                          : HttpContext.Current.Request.QueryString);
            //判断密钥
            //var mySign = Partner.GetInstance().SignPartner(partner, qs);
            //if (mySign.IsNullOrEmpty())
            //    return BLL.APICommon.GetStatusCodeResult(10007);
            //if (sign != mySign)
            //    return BLL.APICommon.GetStatusCodeResult(10009);
            var mdReg = new Regex("^(?<cls>[a-z0-9_]+)\\.(?<md>[a-z0-9\\._]+)$", RegexOptions.IgnoreCase);
            if (!mdReg.IsMatch(method))
            {
                return new AjaxResult {state = 0, msg = "请求无效！"};
            }
            var macth = mdReg.Match(method);
            var cls = macth.Groups["cls"].Value;
            var md = macth.Groups["md"].Value.Replace(".", "_");
            var result = AjaxUtils.ExecutinonMethod("Shoy.Demo", cls, md, HttpContext.Current);
            if (result is AjaxResult)
                return result;
            return new AjaxResult {state = 1, msg = "", result = result};
        }


        [AjaxAction(RequestType.Get)]
        public string methods_list()
        {
            return AjaxUtils.ShowMethods();
        }
    }
}