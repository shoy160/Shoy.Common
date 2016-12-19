using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace DayEasy.God.Services.OnlinePay
{
    public class AlipaySignature
    {
        /** 默认编码字符集 */
        private const string DefaultCharset = "GBK";

        public static string RsaSign(string data, string privateKey, string charset = DefaultCharset,
            string signType = "RSA")
        {
            var rsaCsp = LoadCertificateFile(privateKey, signType);
            var dataBytes = string.IsNullOrEmpty(charset)
                ? Encoding.UTF8.GetBytes(data)
                : Encoding.GetEncoding(charset).GetBytes(data);


            if ("RSA2".Equals(signType))
            {
                var signatureBytes = rsaCsp.SignData(dataBytes, "SHA256");

                return Convert.ToBase64String(signatureBytes);

            }
            else
            {
                var signatureBytes = rsaCsp.SignData(dataBytes, "SHA1");

                return Convert.ToBase64String(signatureBytes);
            }
        }


        public static string RsaDecrypt(string content, string privateKeyPem, string charset, string signType)
        {
            try
            {
                var rsaCsp = LoadCertificateFile(privateKeyPem, signType);
                if (string.IsNullOrEmpty(charset))
                {
                    charset = DefaultCharset;
                }
                var data = Convert.FromBase64String(content);
                var maxBlockSize = rsaCsp.KeySize / 8; //解密块最大长度限制
                if (data.Length <= maxBlockSize)
                {
                    var cipherbytes = rsaCsp.Decrypt(data, false);
                    return Encoding.GetEncoding(charset).GetString(cipherbytes);
                }
                var crypStream = new MemoryStream(data);
                var plaiStream = new MemoryStream();
                var buffer = new byte[maxBlockSize];
                var blockSize = crypStream.Read(buffer, 0, maxBlockSize);
                while (blockSize > 0)
                {
                    var toDecrypt = new byte[blockSize];
                    Array.Copy(buffer, 0, toDecrypt, 0, blockSize);
                    var cryptograph = rsaCsp.Decrypt(toDecrypt, false);
                    plaiStream.Write(cryptograph, 0, cryptograph.Length);
                    blockSize = crypStream.Read(buffer, 0, maxBlockSize);
                }

                return Encoding.GetEncoding(charset).GetString(plaiStream.ToArray());
            }
            catch (Exception ex)
            {
                throw new Exception("DecryptContent = " + content + ",charset = " + charset, ex);
            }
        }

        private static byte[] GetPem(string type, string key)
        {
            string header = $"-----BEGIN {type}-----\\n";
            string footer = $"-----END {type}-----";
            var start = key.IndexOf(header, StringComparison.Ordinal) + header.Length;
            var end = key.IndexOf(footer, start, StringComparison.Ordinal);
            var base64 = key.Substring(start, (end - start));
            return Convert.FromBase64String(base64);
        }

        private static RSACryptoServiceProvider LoadCertificateFile(string privateKey, string signType)
        {
            var res = GetPem("RSA PRIVATE KEY", privateKey);
            try
            {
                var rsa = DecodeRsaPrivateKey(res, signType);
                return rsa;
            }
            catch (Exception ex)
            {
            }
            return null;
        }

        private static RSACryptoServiceProvider DecodeRsaPrivateKey(byte[] privkey, string signType)
        {
            byte[] MODULUS, E, D, P, Q, DP, DQ, IQ;

            // --------- Set up stream to decode the asn.1 encoded RSA private key ------
            MemoryStream mem = new MemoryStream(privkey);
            BinaryReader binr = new BinaryReader(mem);  //wrap Memory Stream with BinaryReader for easy reading
            byte bt = 0;
            ushort twobytes = 0;
            int elems = 0;
            try
            {
                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8130) //data read as little endian order (actual data order for Sequence is 30 81)
                    binr.ReadByte();    //advance 1 byte
                else if (twobytes == 0x8230)
                    binr.ReadInt16();    //advance 2 bytes
                else
                    return null;

                twobytes = binr.ReadUInt16();
                if (twobytes != 0x0102) //version number
                    return null;
                bt = binr.ReadByte();
                if (bt != 0x00)
                    return null;


                //------ all private key components are Integer sequences ----
                elems = GetIntegerSize(binr);
                MODULUS = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                E = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                D = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                P = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                Q = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                DP = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                DQ = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                IQ = binr.ReadBytes(elems);


                // ------- create RSACryptoServiceProvider instance and initialize with public key -----
                var cspParameters = new CspParameters { Flags = CspProviderFlags.UseMachineKeyStore };

                var bitLen = 1024;
                if ("RSA2".Equals(signType))
                {
                    bitLen = 2048;
                }

                var rsa = new RSACryptoServiceProvider(bitLen, cspParameters);
                var rsAparams = new RSAParameters
                {
                    Modulus = MODULUS,
                    Exponent = E,
                    D = D,
                    P = P,
                    Q = Q,
                    DP = DP,
                    DQ = DQ,
                    InverseQ = IQ
                };
                rsa.ImportParameters(rsAparams);
                return rsa;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                binr.Close();
            }
        }

        private static int GetIntegerSize(BinaryReader binr)
        {
            byte bt = 0;
            byte lowbyte = 0x00;
            byte highbyte = 0x00;
            int count = 0;
            bt = binr.ReadByte();
            if (bt != 0x02)		//expect integer
                return 0;
            bt = binr.ReadByte();

            if (bt == 0x81)
                count = binr.ReadByte();	// data size in next byte
            else
                if (bt == 0x82)
            {
                highbyte = binr.ReadByte(); // data size in next 2 bytes
                lowbyte = binr.ReadByte();
                byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };
                count = BitConverter.ToInt32(modint, 0);
            }
            else
            {
                count = bt;     // we already have the data size
            }

            while (binr.ReadByte() == 0x00)
            {	//remove high order zeros in data
                count -= 1;
            }
            binr.BaseStream.Seek(-1, SeekOrigin.Current);		//last ReadByte wasn't a removed zero, so back up a byte
            return count;
        }
    }
}
