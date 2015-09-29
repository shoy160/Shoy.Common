using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Shoy.Utility.Extend;

namespace Shoy.Utility.Helper
{
    public enum NamingType
    {
        /// <summary> 默认命名,UserName </summary>
        Normal = 0,

        /// <summary> 驼峰命名,如：userName </summary>
        CamelCase = 1,

        /// <summary> url命名,如：user_name，注：反序列化时也需要指明 </summary>
        UrlCase = 2
    }

    internal class JsonContractResolver : DefaultContractResolver
    {
        private readonly string[] _props;
        private readonly bool _retain;
        private readonly NamingType _camelCase;

        public JsonContractResolver(NamingType camelCase)
        {
            _camelCase = camelCase;
        }

        /// <summary> 构造函数 </summary>
        /// <param name="camelCase">驼峰</param>
        /// <param name="retain">保留/排除：true为保留</param>
        /// <param name="props"></param>
        public JsonContractResolver(NamingType camelCase = NamingType.Normal, bool retain = true,
            params string[] props)
        {
            _camelCase = camelCase;
            _retain = retain;
            _props = props;
        }

        protected override string ResolvePropertyName(string propertyName)
        {
            switch (_camelCase)
            {
                case NamingType.CamelCase:
                    return propertyName.ToCamelCase();
                case NamingType.UrlCase:
                    return propertyName.ToUrlCase();
                default:
                    return base.ResolvePropertyName(propertyName);
            }
        }

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var propList = base.CreateProperties(type, memberSerialization);
            if (_props == null || _props.Length == 0)
                return propList;
            return
                propList.Where(
                    p => _retain
                        ? _props.Contains(p.PropertyName)
                        : !_props.Contains(p.PropertyName))
                    .ToList();
        }
    }
}
