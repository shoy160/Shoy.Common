using System;

namespace Shoy.Laboratory.Transmiter
{
    /// <summary>
    /// 文件块异常类
    /// </summary>
    public class FileBlockException : Exception
    {
        public enum ErrorCode
        {
            BadHeader,
            BadIndex,
            IllegalFileBlockSize,
            ChecksumError,
        }

        public ErrorCode Code { get; set; }

        public FileBlockException(string message, ErrorCode errorCode)
            : base(message)
        {
            Code = errorCode;
        }
    }
}
