
using System.ComponentModel;

namespace Shoy.Core.Data
{
    /// <summary>
    /// 业务操作结果信息类，对操作结果进行封装
    /// </summary>
    public class OperateResult : OperationResult<object>
    {
        #region 构造函数

        /// <summary>
        /// 初始化一个<see cref="OperateResult"/>类型的新实例
        /// </summary>
        public OperateResult(OperateResultType resultType)
            : this(resultType, null, null)
        { }

        /// <summary>
        /// 初始化一个<see cref="OperateResult"/>类型的新实例
        /// </summary>
        public OperateResult(OperateResultType resultType, string message)
            : this(resultType, message, null)
        { }

        /// <summary>
        /// 初始化一个<see cref="OperateResult"/>类型的新实例
        /// </summary>
        public OperateResult(OperateResultType resultType, string message, object data)
            : base(resultType, message, data)
        { }

        #endregion
    }


    /// <summary>
    /// 泛型版本的业务操作结果信息类，对操作结果进行封装
    /// </summary>
    /// <typeparam name="T">返回数据的类型</typeparam>
    public class OperationResult<T>
    {
        /// <summary>
        /// 初始化一个<see cref="OperationResult{T}"/>类型的新实例
        /// </summary>
        public OperationResult(OperateResultType resultType)
            : this(resultType, null, default(T))
        { }

        /// <summary>
        /// 初始化一个<see cref="OperationResult{T}"/>类型的新实例
        /// </summary>
        public OperationResult(OperateResultType resultType, string message)
            : this(resultType, message, default(T))
        { }

        /// <summary>
        /// 初始化一个<see cref="OperationResult{T}"/>类型的新实例
        /// </summary>
        public OperationResult(OperateResultType resultType, string message, T data)
        {
            ResultType = resultType;
            Message = message;
            Data = data;
        }

        /// <summary>
        /// 获取或设置 操作结果类型
        /// </summary>
        public OperateResultType ResultType { get; set; }

        /// <summary>
        /// 获取或设置 操作返回消息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 获取或设置 操作返回数据
        /// </summary>
        public T Data { get; set; }
    }

    /// <summary>
    /// 表示业务操作结果的枚举
    /// </summary>
    public enum OperateResultType
    {
        /// <summary>
        ///   输入信息验证失败
        /// </summary>
        [Description("输入信息验证失败！")]
        ValidError,

        /// <summary>
        ///   指定参数的数据不存在
        /// </summary>
        [Description("指定参数的数据不存在！")]
        QueryNull,

        /// <summary>
        ///   操作取消或操作没引发任何变化
        /// </summary>
        [Description("操作没有引发任何变化，提交取消！")]
        NoChanged,

        /// <summary>
        ///   操作成功
        /// </summary>
        [Description("操作成功！")]
        Success,

        /// <summary>
        ///   操作引发错误
        /// </summary>
        [Description("操作引发错误！")]
        Error
    }
}