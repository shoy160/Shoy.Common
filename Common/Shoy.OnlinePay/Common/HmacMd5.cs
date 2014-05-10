using System;

namespace Shoy.OnlinePay.Common
{
    /// <summary>
    /// HmacMD5加密
    /// </summary>

    public class HmacMd5
    {
        private readonly uint[] _count;
        private readonly uint[] _state;
        private readonly byte[] _buffer;
        private byte[] _digest;

        public HmacMd5()
        {
            _count = new uint[2];
            _state = new uint[4];
            _buffer = new byte[64];
            _digest = new byte[16];
            Init();
        }

        public void Init()
        {
            _count[0] = 0;
            _count[1] = 0;
            _state[0] = 0x67452301;
            _state[1] = 0xefcdab89;
            _state[2] = 0x98badcfe;
            _state[3] = 0x10325476;
        }

        public void Update(byte[] data, uint length)
        {
            uint left = length;
            uint offset = (_count[0] >> 3) & 0x3F;
            var bitLength = length << 3;
            uint index = 0;

            if (length <= 0)
                return;

            _count[0] += bitLength;
            _count[1] += (length >> 29);
            if (_count[0] < bitLength)
                _count[1]++;

            if (offset > 0)
            {
                uint space = 64 - offset;
                uint copy = (offset + length > 64 ? 64 - offset : length);
                Buffer.BlockCopy(data, 0, _buffer, (int)offset, (int)copy);

                if (offset + copy < 64)
                    return;

                Transform(_buffer);
                index += copy;
                left -= copy;
            }

            for (; left >= 64; index += 64, left -= 64)
            {
                Buffer.BlockCopy(data, (int)index, _buffer, 0, 64);
                Transform(_buffer);
            }

            if (left > 0)
                Buffer.BlockCopy(data, (int)index, _buffer, 0, (int)left);

        }

        private static readonly byte[] Pad = new byte[]
                                                 {
                                                     0x80, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                                                     0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                                                     0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                                                     0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
                                                 };
        public byte[] Finalize()
        {
            var bits = new byte[8];
            Encode(ref bits, _count, 8);
            var index = (_count[0] >> 3) & 0x3f;
            uint padLen = (index < 56) ? (56 - index) : (120 - index);
            Update(Pad, padLen);
            Update(bits, 8);
            Encode(ref _digest, _state, 16);

            for (int i = 0; i < 64; i++)
                _buffer[i] = 0;

            return _digest;
        }

        public string Md5String()
        {
            string s = "";

            for (int i = 0; i < _digest.Length; i++)
                s += _digest[i].ToString("x2");

            return s;
        }

        #region Constants for MD5Transform routine.

        private const uint S11 = 7;
        private const uint S12 = 12;
        private const uint S13 = 17;
        private const uint S14 = 22;
        private const uint S21 = 5;
        private const uint S22 = 9;
        private const uint S23 = 14;
        private const uint S24 = 20;
        private const uint S31 = 4;
        private const uint S32 = 11;
        private const uint S33 = 16;
        private const uint S34 = 23;
        private const uint S41 = 6;
        private const uint S42 = 10;
        private const uint S43 = 15;
        private const uint S44 = 21;
        #endregion

        private void Transform(byte[] data)
        {
            uint a = _state[0];
            uint b = _state[1];
            uint c = _state[2];
            uint d = _state[3];
            var x = new uint[16];

            Decode(ref x, data, 64);

            // Round 1
            Ff(ref a, b, c, d, x[0], S11, 0xd76aa478); /* 1 */
            Ff(ref d, a, b, c, x[1], S12, 0xe8c7b756); /* 2 */
            Ff(ref c, d, a, b, x[2], S13, 0x242070db); /* 3 */
            Ff(ref b, c, d, a, x[3], S14, 0xc1bdceee); /* 4 */
            Ff(ref a, b, c, d, x[4], S11, 0xf57c0faf); /* 5 */
            Ff(ref d, a, b, c, x[5], S12, 0x4787c62a); /* 6 */
            Ff(ref c, d, a, b, x[6], S13, 0xa8304613); /* 7 */
            Ff(ref b, c, d, a, x[7], S14, 0xfd469501); /* 8 */
            Ff(ref a, b, c, d, x[8], S11, 0x698098d8); /* 9 */
            Ff(ref d, a, b, c, x[9], S12, 0x8b44f7af); /* 10 */
            Ff(ref c, d, a, b, x[10], S13, 0xffff5bb1); /* 11 */
            Ff(ref b, c, d, a, x[11], S14, 0x895cd7be); /* 12 */
            Ff(ref a, b, c, d, x[12], S11, 0x6b901122); /* 13 */
            Ff(ref d, a, b, c, x[13], S12, 0xfd987193); /* 14 */
            Ff(ref c, d, a, b, x[14], S13, 0xa679438e); /* 15 */
            Ff(ref b, c, d, a, x[15], S14, 0x49b40821); /* 16 */

            // Round 2 
            Gg(ref a, b, c, d, x[1], S21, 0xf61e2562); /* 17 */
            Gg(ref d, a, b, c, x[6], S22, 0xc040b340); /* 18 */
            Gg(ref c, d, a, b, x[11], S23, 0x265e5a51); /* 19 */
            Gg(ref b, c, d, a, x[0], S24, 0xe9b6c7aa); /* 20 */
            Gg(ref a, b, c, d, x[5], S21, 0xd62f105d); /* 21 */
            Gg(ref d, a, b, c, x[10], S22, 0x2441453); /* 22 */
            Gg(ref c, d, a, b, x[15], S23, 0xd8a1e681); /* 23 */
            Gg(ref b, c, d, a, x[4], S24, 0xe7d3fbc8); /* 24 */
            Gg(ref a, b, c, d, x[9], S21, 0x21e1cde6); /* 25 */
            Gg(ref d, a, b, c, x[14], S22, 0xc33707d6); /* 26 */
            Gg(ref c, d, a, b, x[3], S23, 0xf4d50d87); /* 27 */
            Gg(ref b, c, d, a, x[8], S24, 0x455a14ed); /* 28 */
            Gg(ref a, b, c, d, x[13], S21, 0xa9e3e905); /* 29 */
            Gg(ref d, a, b, c, x[2], S22, 0xfcefa3f8); /* 30 */
            Gg(ref c, d, a, b, x[7], S23, 0x676f02d9); /* 31 */
            Gg(ref b, c, d, a, x[12], S24, 0x8d2a4c8a); /* 32 */

            // Round 3
            Hh(ref a, b, c, d, x[5], S31, 0xfffa3942); /* 33 */
            Hh(ref d, a, b, c, x[8], S32, 0x8771f681); /* 34 */
            Hh(ref c, d, a, b, x[11], S33, 0x6d9d6122); /* 35 */
            Hh(ref b, c, d, a, x[14], S34, 0xfde5380c); /* 36 */
            Hh(ref a, b, c, d, x[1], S31, 0xa4beea44); /* 37 */
            Hh(ref d, a, b, c, x[4], S32, 0x4bdecfa9); /* 38 */
            Hh(ref c, d, a, b, x[7], S33, 0xf6bb4b60); /* 39 */
            Hh(ref b, c, d, a, x[10], S34, 0xbebfbc70); /* 40 */
            Hh(ref a, b, c, d, x[13], S31, 0x289b7ec6); /* 41 */
            Hh(ref d, a, b, c, x[0], S32, 0xeaa127fa); /* 42 */
            Hh(ref c, d, a, b, x[3], S33, 0xd4ef3085); /* 43 */
            Hh(ref b, c, d, a, x[6], S34, 0x4881d05); /* 44 */
            Hh(ref a, b, c, d, x[9], S31, 0xd9d4d039); /* 45 */
            Hh(ref d, a, b, c, x[12], S32, 0xe6db99e5); /* 46 */
            Hh(ref c, d, a, b, x[15], S33, 0x1fa27cf8); /* 47 */
            Hh(ref b, c, d, a, x[2], S34, 0xc4ac5665); /* 48 */

            // Round 4
            Ii(ref a, b, c, d, x[0], S41, 0xf4292244); /* 49 */
            Ii(ref d, a, b, c, x[7], S42, 0x432aff97); /* 50 */
            Ii(ref c, d, a, b, x[14], S43, 0xab9423a7); /* 51 */
            Ii(ref b, c, d, a, x[5], S44, 0xfc93a039); /* 52 */
            Ii(ref a, b, c, d, x[12], S41, 0x655b59c3); /* 53 */
            Ii(ref d, a, b, c, x[3], S42, 0x8f0ccc92); /* 54 */
            Ii(ref c, d, a, b, x[10], S43, 0xffeff47d); /* 55 */
            Ii(ref b, c, d, a, x[1], S44, 0x85845dd1); /* 56 */
            Ii(ref a, b, c, d, x[8], S41, 0x6fa87e4f); /* 57 */
            Ii(ref d, a, b, c, x[15], S42, 0xfe2ce6e0); /* 58 */
            Ii(ref c, d, a, b, x[6], S43, 0xa3014314); /* 59 */
            Ii(ref b, c, d, a, x[13], S44, 0x4e0811a1); /* 60 */
            Ii(ref a, b, c, d, x[4], S41, 0xf7537e82); /* 61 */
            Ii(ref d, a, b, c, x[11], S42, 0xbd3af235); /* 62 */
            Ii(ref c, d, a, b, x[2], S43, 0x2ad7d2bb); /* 63 */
            Ii(ref b, c, d, a, x[9], S44, 0xeb86d391); /* 64 */

            _state[0] += a;
            _state[1] += b;
            _state[2] += c;
            _state[3] += d;

            for (int i = 0; i < 16; i++)
                x[i] = 0;
        }

        #region encode - decode
        private static void Encode(ref byte[] output, uint[] input, uint len)
        {
            uint i, j;
            if (BitConverter.IsLittleEndian)
            {
                for (i = 0, j = 0; j < len; i++, j += 4)
                {
                    output[j] = (byte)(input[i] & 0xff);
                    output[j + 1] = (byte)((input[i] >> 8) & 0xff);
                    output[j + 2] = (byte)((input[i] >> 16) & 0xff);
                    output[j + 3] = (byte)((input[i] >> 24) & 0xff);
                }
            }
            else
            {
                for (i = 0, j = 0; j < len; i++, j += 4)
                {
                    output[j + 3] = (byte)(input[i] & 0xff);
                    output[j + 2] = (byte)((input[i] >> 8) & 0xff);
                    output[j + 1] = (byte)((input[i] >> 16) & 0xff);
                    output[j] = (byte)((input[i] >> 24) & 0xff);
                }
            }
        }

        private static void Decode(ref uint[] output, byte[] input, uint len)
        {
            uint i, j;
            if (BitConverter.IsLittleEndian)
            {
                for (i = 0, j = 0; j < len; i++, j += 4)
                    output[i] = input[j] | (((uint)input[j + 1]) << 8) |
                        (((uint)input[j + 2]) << 16) | (((uint)input[j + 3]) << 24);
            }
            else
            {
                for (i = 0, j = 0; j < len; i++, j += 4)
                    output[i] = input[j + 3] | (((uint)input[j + 2]) << 8) |
                        (((uint)input[j + 1]) << 16) | (((uint)input[j]) << 24);
            }
        }
        #endregion

        private static uint RotateLeft(uint x, uint n)
        {
            return (x << (int)n) | (x >> (int)(32 - n));
        }

        #region F, G, H and I are basic MD5 functions.
        private static uint F(uint x, uint y, uint z)
        {
            return (x & y) | (~x & z);
        }

        private static uint G(uint x, uint y, uint z)
        {
            return (x & z) | (y & ~z);
        }

        private static uint H(uint x, uint y, uint z)
        {
            return x ^ y ^ z;
        }

        private static uint I(uint x, uint y, uint z)
        {
            return y ^ (x | ~z);
        }
        #endregion

        #region  FF, GG, HH, and II transformations for rounds 1, 2, 3, and 4.
        private static void Ff(ref uint a, uint b, uint c, uint d, uint x, uint s, uint ac)
        {
            a += F(b, c, d) + x + ac;
            a = RotateLeft(a, s) + b;
        }

        private static void Gg(ref uint a, uint b, uint c, uint d, uint x, uint s, uint ac)
        {
            a += G(b, c, d) + x + ac;
            a = RotateLeft(a, s) + b;
        }

        private static void Hh(ref uint a, uint b, uint c, uint d, uint x, uint s, uint ac)
        {
            a += H(b, c, d) + x + ac;
            a = RotateLeft(a, s) + b;
        }

        private static void Ii(ref uint a, uint b, uint c, uint d, uint x, uint s, uint ac)
        {
            a += I(b, c, d) + x + ac;
            a = RotateLeft(a, s) + b;
        }
        #endregion
    }
}
