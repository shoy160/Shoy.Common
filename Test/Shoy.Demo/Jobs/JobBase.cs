using Shoy.Backgrounder;
using Shoy.Utility;
using Shoy.Utility.Helper;
using System;
using System.IO;

namespace Shoy.Demo.Jobs
{
    public abstract class JobBase : Job
    {
        private string FilePath
        {
            get { return Utils.GetAppSetting(null, string.Empty); }
        }

        public void Write(string msg)
        {
            string directory = Path.GetDirectoryName(FilePath);
            if (string.IsNullOrWhiteSpace(directory))
                return;
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            FileHelper.WriteFile(FilePath,
                string.Format("Time:{0}{1}Message:{2}{1}", Utils.GetTimeNow(), Environment.NewLine, msg));
        }

        protected JobBase(string name, TimeSpan interval, TimeSpan timeout, DateTime? start = null, DateTime? expire = null)
            : base(name, interval, timeout, start, expire)
        {
        }
    }
}