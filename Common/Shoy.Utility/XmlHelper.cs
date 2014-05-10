using System.IO;
using System.Xml.Serialization;
using Shoy.Utility.Extend;

namespace Shoy.Utility
{
    public class XmlHelper
    {
        /// <summary>
        /// xml反序列化
        /// </summary>
        /// <typeparam name="T">xml序列化类型</typeparam>
        /// <param name="path">xml文件路径</param>
        /// <param name="msg">返回信息</param>
        /// <returns></returns>
        public static T XmlDeserialize<T>(string path, out string msg)
        {
            msg = "";
            if (!File.Exists(path))
                return default(T);
            FileStream fs = null;
            try
            {
                fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                var serializer = new XmlSerializer(typeof (T));
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

        /// <summary>
        /// xml反序列化
        /// </summary>
        /// <typeparam name="T">xml序列化类型</typeparam>
        /// <param name="path">xml文件路径</param>
        /// <returns></returns>
        public static T XmlDeserialize<T>(string path)
        {
            string msg;
            return XmlDeserialize<T>(path, out msg);
        }

        /// <summary>
        /// xml序列化
        /// </summary>
        /// <param name="path">xml文件路径</param>
        /// <param name="obj">序列化对象</param>
        /// <param name="msg">返回信息</param>
        /// <returns></returns>
        public static bool XmlSerialize(string path, object obj,out string msg)
        {
            msg = "";
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

        /// <summary>
        /// xml序列化
        /// </summary>
        /// <param name="path">xml文件路径</param>
        /// <param name="obj">序列化对象</param>
        /// <returns></returns>
        public static bool XmlSerialize(string path, object obj)
        {
            string msg;
            return XmlSerialize(path, obj, out msg);
        }
    }
}
