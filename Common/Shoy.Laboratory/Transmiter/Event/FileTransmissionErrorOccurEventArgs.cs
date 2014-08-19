using System;

namespace Shoy.Laboratory.Transmiter
{
    public class FileTransmissionErrorOccurEventArgs : EventArgs
    {
        public Exception InnerException { get; set; }
        /// <summary>
        /// 指示是否继续运行
        /// </summary>
        public bool Continue { get; set; }
        public FileTransmissionErrorOccurEventArgs(Exception innerException)
        {
            InnerException = innerException;
            Continue = false;
        }
    }
}
