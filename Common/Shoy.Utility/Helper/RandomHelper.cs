using System;
using System.Linq;
using System.Text;

namespace Shoy.Utility.Helper
{
    /// <summary>
    /// 随机数辅助
    /// </summary>
    public static class RandomHelper
    {
        private const string AllLetter = "mnbvcxzlkjhgfdsapoiuytrewq";
        private const string HardWord = "0oOz29q1ilI6b";
        private const string AllWord = "qwNOPerWXYktyu421ioKfdsS867plVjMZ9hgDEnbTUxcABGHIJaCFmL0vzQR53";
        private const string HexWords = "0,1,2,3,4,5,6,7,8,9,a,b,c,d,e,f";

        /// <summary>
        /// 获取线程级随机数
        /// </summary>
        /// <returns></returns>
        public static Random Random()
        {
            var bytes = new byte[4];
            var rng =
                new System.Security.Cryptography.RNGCryptoServiceProvider();
            rng.GetBytes(bytes);
            var seed = BitConverter.ToInt32(bytes, 0);
            var tick = DateTime.Now.Ticks + (seed);
            return new Random((int)(tick & 0xffffffffL) | (int)(tick >> 32));
        }

        /// <summary>
        /// 随机数字
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string RandomNums(int length)
        {
            if (length <= 0) return string.Empty;
            var sb = new StringBuilder();
            var random = Random();
            for (var i = 0; i < length; i++)
            {
                sb.Append(random.Next(0, 9));
            }
            return sb.ToString();
        }

        /// <summary>
        /// 随机数字
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string RandomLetters(int length)
        {
            if (length <= 0) return string.Empty;
            var sb = new StringBuilder();
            var random = Random();
            for (var i = 0; i < length; i++)
            {
                sb.Append(AllLetter[random.Next(0, AllLetter.Length - 1)]);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 获取指定长度的随机字符串
        /// </summary>
        /// <param name="length">长度</param>
        /// <param name="hardWord">是否包含难辨别字符</param>
        /// <returns></returns>
        public static string RandomNumAndLetters(int length, bool hardWord = false)
        {
            if (length <= 0) return string.Empty;
            var list = (hardWord ? AllWord.ToArray() : AllWord.ToArray().Except(HardWord.ToArray())).ToArray();
            var random = Random();
            var sb = new StringBuilder();
            for (var i = 0; i < length; i++)
            {
                sb.Append(list[random.Next(0, list.Length - 1)]);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 获取随机汉字
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string RandomHanzi(int length)
        {
            if (length <= 0) return string.Empty;
            //汉字编码的组成元素，十六进制数
            var baseStrs = HexWords.Split(',');
            var encoding = Encoding.GetEncoding("GB2312");
            string result = null;

            //每循环一次产生一个含两个元素的十六进制字节数组，并放入bytes数组中
            //汉字由四个区位码组成，1、2位作为字节数组的第一个元素，3、4位作为第二个元素
            var rnd = Random();
            for (int i = 0; i < length; i++)
            {
                int index1 = rnd.Next(11, 14);
                string str1 = baseStrs[index1];

                int index2 = index1 == 13 ? rnd.Next(0, 7) : rnd.Next(0, 16);
                string str2 = baseStrs[index2];

                int index3 = rnd.Next(10, 16);
                string str3 = baseStrs[index3];

                int index4 = index3 == 10 ? rnd.Next(1, 16) : (index3 == 15 ? rnd.Next(0, 15) : rnd.Next(0, 16));
                string str4 = baseStrs[index4];

                //定义两个字节变量存储产生的随机汉字区位码
                byte b1 = Convert.ToByte(str1 + str2, 16);
                byte b2 = Convert.ToByte(str3 + str4, 16);
                byte[] bs = { b1, b2 };

                result += encoding.GetString(bs);
            }
            return result;
        }
    }
}
