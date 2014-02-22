using System.IO;
using System.Linq;

namespace Shoy.Data.Core
{
    public class Functions
    {
        private Functions() { }

        public static long DirSize(DirectoryInfo info)
        {
            FileInfo[] fis = info.GetFiles();
            long size = fis.Sum(fi => fi.Length);
            // Add subdirectory sizes.
            DirectoryInfo[] dis = info.GetDirectories();
            size += dis.Sum(di => DirSize(di));
            return (size);
        }
    }
}
