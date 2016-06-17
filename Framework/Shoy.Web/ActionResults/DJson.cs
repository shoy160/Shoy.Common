using System;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Shoy.Utility.Helper;

namespace Shoy.Web.ActionResults
{
    public class DJson : ActionResult
    {
        private readonly object _data;

        /// <summary> 组织GET方法 </summary>
        private readonly bool _denyGet;

        private readonly string[] _props;
        private readonly bool _retain;
        private readonly NamingType _namingType;

        private DJson(object data, bool denyGet, bool retain, string[] props, NamingType namingType)
        {
            _data = data;
            _denyGet = denyGet;
            _props = props;
            _retain = retain;
            _namingType = namingType;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            if (_denyGet &&
                String.Equals(context.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "此请求已被阻止，因为当用在 GET 请求中时，会将敏感信息透漏给第三方网站。若要允许 GET 请求，请将 DenyGet 设置为 false");
            }

            HttpResponseBase response = context.HttpContext.Response;
            response.ContentType = "application/json";
            response.ContentEncoding = Encoding.UTF8;
            if (_data != null)
            {
#if DEBUG
                response.Write(JsonHelper.ToJson(_data, _namingType, true, retain: _retain, props: _props));
#else
                response.Write(JsonHelper.ToJson(_data, _namingType, retain: _retain, props: _props));
#endif
            }
        }

        public static DJson Json(object data, bool denyGet = false, bool retain = true, NamingType namingType = NamingType.Normal, params string[] props)
        {
            return new DJson(data, denyGet, retain, props, namingType);
        }
    }
}
