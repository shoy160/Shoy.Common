using System;
using System.Text;

namespace ShoyConsoleTest
{
    public class ResharperKey
    {
        private const string PUBLIC_KEY = "3439664563558343388897268028516178220150974083190422162869";
        private const string PRIVATE_KEY = "";

        public static string GetLicense(string userName, string company)
        {
            int iUserHash = UserHash(userName, company);

            // 基础的Seed，即找到Endless License的明文
            var bigBaseSeed = new BigInteger((long) 65535);
            bigBaseSeed <<= 56; // 校验代码中向右移56位，我就向左移56位
            bigBaseSeed += new BigInteger((long) iUserHash); // 为了使IsChecksum返回true

            // Public Key 即充当N
            var bigPublickKey = new BigInteger(PUBLIC_KEY, 10);

            // Private Key 即充当Z
            var bigPrivateKey = new BigInteger(PRIVATE_KEY, 10);

            // 用户名计算 即充当E
            var bigUserName = new BigInteger(Encoding.Default.GetBytes(userName));
            bigUserName |= 1;

            // 搞定最重要的密钥D
            BigInteger bigD = bigUserName.modInverse(bigPrivateKey);

            // 套加密公式 ci = n^d mod n
            BigInteger bigLicense = bigBaseSeed.modPow(bigD, bigPublickKey);


            return (Convert.ToBase64String(bigLicense.getBytes()));
        }

        // UserHash ，直接从反编译的源代码中抄过来的
        // 作用是判断用户名与License是否匹配
        private static int UserHash(string userName, string company)
        {
            int i = 0;
            for (int j = 0; j < userName.Length; j++)
            {
                i = ((i << 7) + userName[j])%65521;
            }

            for (int k = 0; k < company.Length; k++)
            {
                i = ((i << 7) + company[k])%65521;
            }
            return (i);
        }
    }
}
