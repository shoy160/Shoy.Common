using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Shoy.Utility.Helper;

namespace Shoy.Utility.License
{
    /// <summary> 编码/激活码管理 </summary>
    public class LicenseHelper
    {
        private readonly int _codeLength;
        private List<string> _cacheCodes;
        private readonly LicenseType _licenseType;
        private Func<int, List<string>, int, string> _generateRole;

        internal LicenseHelper(LicenseType type)
        {
            _licenseType = type;
            _codeLength = 8;
            var codeLength = type.GetType().GetField(type.ToString()).GetCustomAttribute<CodeLengthAttribute>();
            if (codeLength != null)
                _codeLength = codeLength.Length;
            _cacheCodes = new List<string>();
        }

        internal void SetGenerateRole(Func<int, List<string>, int, string> role)
        {
            _generateRole = role;
        }

        internal void SetCache(IEnumerable<string> caches)
        {
            _cacheCodes.AddRange(caches);
            _cacheCodes = _cacheCodes.Distinct().ToList();
        }

        private string GenerateCode(string prefix = null, int tryCount = 0)
        {
            if (_generateRole != null)
                return _generateRole(_codeLength, _cacheCodes, tryCount);
            var len = _codeLength;
            if (!string.IsNullOrWhiteSpace(prefix))
                len -= prefix.Length;
            if (len <= 0 || len > 64)
                return string.Empty;
            var code = IdHelper.Instance.Guid32;
            if (len > 32)
                code = string.Concat(code, IdHelper.Instance.Guid32);
            return (prefix ?? string.Empty) + code.Substring(0, len);
        }

        /// <summary> 使用编码/激活码 </summary>
        /// <param name="code"></param>
        public void Used(string code)
        {
            _cacheCodes.Remove(code);
        }

        /// <summary> 生成编码/激活码 </summary>
        /// <param name="prefix">前缀，会在总长度上减去前缀的长度</param>
        /// <returns></returns>
        public string Code(string prefix = null)
        {
            var code = GenerateCode(prefix);
            int count = 0;
            while (_cacheCodes.Contains(code))
            {
                count++;
                code = GenerateCode(prefix, count);
            }
            _cacheCodes.Add(code);
            return code;
        }
    }
}
