
using System;

namespace Shoy.Wpf.Logging.Log4Net
{
    [Serializable]
    public class LogInfo
    {
        public string SiteName { get; set; }
        public string Method { get; set; }
        public string File { get; set; }
        public string Message { get; set; }
        public string Detail { get; set; }

        public override string ToString()
        {
            return string.Format("site:{5} file:{0}{4}method:{1}{4}message:{2}{4}detail:{3}", File, Method, Message, Detail,
                Environment.NewLine, SiteName);
        }
    }
}
