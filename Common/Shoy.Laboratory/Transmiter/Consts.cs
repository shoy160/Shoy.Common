using System;
using System.Linq;
using System.Text;

namespace Shoy.Laboratory.Transmiter
{
    /// <summary>
    /// 一些常量和扩展方法
    /// </summary>
    public static class Consts
    {
        /// <summary>
        /// 文件区块数据标头
        /// </summary>
        public const byte FileBlockHeader = 0;

        /// <summary>
        /// 字符串信息标头
        /// </summary>
        public const byte StringHeader = 1;

        /// <summary>
        /// 分块大小1MB
        /// </summary>
        public const int BlockSize = 1048576;

        /// <summary>
        /// 网络上传送的数据包最大大小
        /// </summary>
        public const int NetBlockMaxSize = BlockSize + 9;

        /// <summary>
        /// 默认磁盘缓存大小(单位:区块数)
        /// </summary>
        public const int DefaultIoBufferSize = 8;

        /// <summary>
        /// 空格
        /// </summary>
        public const string Space = " ";

        /// <summary>
        /// 空格替代符
        /// </summary>
        public const string SpaceReplacement = @"<SPACE>";

        /// <summary>
        /// 获取校验值
        /// </summary>
        /// <param name="bytes">输入数据</param>
        /// <returns>输出的校验值</returns>
        public static byte[] GetHash(this byte[] bytes)
        {
            return BitConverter.GetBytes(Crc32.GetCrc32(bytes));
        }

        /// <summary>
        /// 比较两二进制数据内容是否完全相同(用于MD5值的比较)
        /// </summary>
        /// <param name="THIS">数据一</param>
        /// <param name="obj">数据二</param>
        public static bool BytesEqual(this byte[] THIS, byte[] obj)
        {
            if (THIS.Length != obj.Length)
                return false;
            return !obj.Where((t, index) => THIS[index] != t).Any();
        }

        /// <summary>
        /// 将指令字符串转化为二进制数据并添加标头
        /// </summary>
        public static byte[] ToBytes(this string strInput)
        {
            byte[] strdata = Encoding.UTF8.GetBytes(strInput);
            var output = new byte[1 + strdata.Length];
            output[0] = StringHeader;
            Array.Copy(strdata, 0, output, 1, strdata.Length);
            return output;
        }

        /// <summary>
        /// 将二进制数据转化为指令字符串
        /// </summary>
        public static string ToFtString(this byte[] bytesInput)
        {
            if (bytesInput[0] != StringHeader)
                throw new FormatException("Bad Header!");
            return Encoding.UTF8.GetString(bytesInput, 1, bytesInput.Length - 1).TrimEnd('\0');
        }

        /// <summary>
        /// 替换可能会对命令解析造成干扰的字符
        /// </summary>
        public static string DoReplace(this string strInput)
        {
            return strInput.Replace(Space, SpaceReplacement);
        }

        /// <summary>
        /// 还原被替换的字符
        /// </summary>
        public static string DeReplace(this string strInput)
        {
            return strInput.Replace(SpaceReplacement, Space);
        }
    }
}
