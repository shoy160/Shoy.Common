
namespace Shoy.Core.Context
{
    /// <summary>
    /// 当前操作者信息类
    /// </summary>
    public class Operator
    {
        /// <summary>
        /// 获取或设置 当前用户标识
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 获取或设置 当前用户名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 获取或设置 当前访问IP
        /// </summary>
        public string Ip { get; set; }
    }
}
