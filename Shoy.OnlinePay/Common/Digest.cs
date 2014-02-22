using System.Text;
using System;
using System.Security.Cryptography;

namespace Shoy.OnlinePay.Common
{
    /// <summary>
    /// Digest 类 
    /// </summary>
    public abstract class Digest
    {
        public static string HmacSign(string aValue, string aKey)
        {
            var kIpad = new byte[64];
            var kOpad = new byte[64];
            byte[] keyb = Encoding.UTF8.GetBytes(aKey);
            byte[] value = Encoding.UTF8.GetBytes(aValue);

            for (int i = keyb.Length; i < 64; i++)
                kIpad[i] = 54;

            for (int i = keyb.Length; i < 64; i++)
                kOpad[i] = 92;

            for (int i = 0; i < keyb.Length; i++)
            {
                kIpad[i] = (byte)(keyb[i] ^ 0x36);
                kOpad[i] = (byte)(keyb[i] ^ 0x5c);
            }

            var md = new HmacMd5();

            md.Update(kIpad, (uint)kIpad.Length);
            md.Update(value, (uint)value.Length);
            byte[] dg = md.Finalize();
            md.Init();
            md.Update(kOpad, (uint)kOpad.Length);
            md.Update(dg, 16);
            dg = md.Finalize();

            return ToHex(dg);
        }

        public static string ToHex(byte[] input)
        {
            if (input == null)
                return null;

            var output = new StringBuilder(input.Length * 2);

            foreach (byte t in input)
            {
                int current = t & 0xff;
                if (current < 16)
                    output.Append("0");
                output.Append(current.ToString("x"));
            }

            return output.ToString();
        }

        /// <summary>
        /// 冒泡排序法
        /// 按照字母序列从a到z的顺序排列
        /// </summary>
        public static string[] BubbleSort(string[] r)
        {
            int i; //交换标志 

            for (i = 0; i < r.Length; i++) //最多做R.Length-1趟排序 
            {
                bool exchange = false;

                int j; //交换标志 
                for (j = r.Length - 2; j >= i; j--)
                {
                    //交换条件
                    if (String.CompareOrdinal(r[j + 1], r[j]) < 0)
                    {
                        string temp = r[j + 1];
                        r[j + 1] = r[j];
                        r[j] = temp;

                        exchange = true; //发生了交换，故将交换标志置为真 
                    }
                }

                if (!exchange) //本趟排序未发生交换，提前终止算法 
                {
                    break;
                }
            }
            return r;
        }

        /// <summary>
        /// 与ASP兼容的MD5加密算法
        /// </summary>
        public static string GetMd5(string s, string inputCharset)
        {
            var encoding = Encoding.GetEncoding(inputCharset);
            return GetMd5(s, encoding);
        }

        public static string GetMd5(string s, Encoding encoding)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] t = md5.ComputeHash(encoding.GetBytes(s));
            var sb = new StringBuilder(32);
            for (int i = 0; i < t.Length; i++)
            {
                sb.Append(t[i].ToString("x").PadLeft(2, '0'));
            }
            return sb.ToString();
        }

        internal static string Md5Hash(string input, Encoding encoding)
        {
            MD5 md5Hasher = MD5.Create();
            if (encoding == null)
            {
                encoding = Encoding.Default;
            }
            byte[] data = md5Hasher.ComputeHash(encoding.GetBytes(input));
            var sBuilder = new StringBuilder();
            int len = data.Length - 1;
            for (int i = 0; i <= len; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }
    }
}
