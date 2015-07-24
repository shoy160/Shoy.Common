using System.ComponentModel;

namespace Shoy.Utility.License
{
    /// <summary> 编码/激活码类型 </summary>
    public enum LicenseType
    {
        /// <summary> 登录码 </summary>
        [Description("登录码")]
        [CodeLength(9)]
        LoginCode = 0
    }
}
