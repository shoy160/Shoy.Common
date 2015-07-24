using System;

namespace Shoy.Services
{
    /// <summary> 服务接口 </summary>
    public interface IMission : IDisposable
    {
        /// <summary> 执行 </summary>
        void Action();

        /// <summary> 中断 </summary>
        void Abort();

        event ErrorHandler Error;
    }

    /// <summary> 错误事件 </summary>
    public class ErrorEventArg
    {
        public ErrorEventArg()
        {
            Message = string.Empty;
            Cancel = false;
        }

        /// <summary> 事件消息 </summary>
        public string Message { get; set; }

        /// <summary> 是否取消 </summary>
        public bool Cancel { get; set; }
    }

    public delegate void ErrorHandler(object sender, ErrorEventArg arg);
}
