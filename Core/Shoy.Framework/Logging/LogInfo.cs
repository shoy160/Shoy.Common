
using System;

namespace Shoy.Framework.Logging
{
    [Serializable]
    public class LogInfo
    {
        public string Method { get; set; }
        public string File { get; set; }
        public string Message { get; set; }
        public string Detail { get; set; }

        public override string ToString()
        {
            return string.Format("file:{0}{4}method:{1}{4}message:{2}{4}detail:{3}", File, Method, Message, Detail,
                Environment.NewLine);
        }
    }
}
