using Shoy.Utility.Extend;
namespace Shoy.AjaxHelper.Model
{
    /// <summary>
    /// 方法的一些路径信息
    /// </summary>
    internal class MethodPathInfo
    {
        /// <summary>
        /// 程序集
        /// </summary>
        public string Assembly { get; set; }

        /// <summary>
        /// 类名称
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// 方法名称
        /// </summary>
        public string MethodName { get; set; }

        public string TypeName { get { return "{0}.{1}".FormatWith(Assembly, ClassName); } }

        public string MethodKey { get { return "{0}.{1}.{2}".FormatWith(Assembly, ClassName, MethodName); } }

        /// <summary>
        /// 这个路径信息是否是有效的 表面判断 如果有空值 则无效
        /// </summary>
        public bool IsValidate
        {
            get {
                return !string.IsNullOrEmpty(Assembly) && !string.IsNullOrEmpty(ClassName) && !string.IsNullOrEmpty(MethodName);
            }
        }


    }
}
