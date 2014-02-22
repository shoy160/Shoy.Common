using System.IO;
using System.Xml.Serialization;
using Shoy.Utility.Extend;

namespace Shoy.Utility
{
    public class XmlHelper
    {
        public static T XmlDeserialize<T>(string path)
        {
            if (!File.Exists(path))
                return default(T);
            FileStream fs = null;
            try
            {
                fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                var serializer = new XmlSerializer(typeof(T));
                return serializer.Deserialize(fs).ObjectToT<T>();
            }
            catch
            {
                return default(T);
            }
            finally
            {
                if (fs != null)
                    fs.Close();
            }
        }

        public static bool XmlSerialize(string path, object obj)
        {
            if (obj == null) return false;
            var directory = Path.GetDirectoryName(path);
            if (directory == null || !Directory.Exists(directory))
                return false;
            FileStream fs = null;
            var tmp = path + ".tmp";
            try
            {
                var serializer = new XmlSerializer(obj.GetType());
                fs = new FileStream(tmp, FileMode.OpenOrCreate, FileAccess.Write);
                serializer.Serialize(fs, obj);
            }
            catch
            {
                return false;
            }
            finally
            {
                if (fs != null)
                    fs.Close();
            }
            return Utils.MoveFile(tmp, path, true);
        }
    }
}
