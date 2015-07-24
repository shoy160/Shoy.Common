using System;
using System.Collections.Generic;
using System.Globalization;

namespace Shoy.Utility.Helper
{
    /// <summary>
    /// ID生成工具
    /// </summary>
    public class IdHelper
    {
        public static IdHelper Instance
        {
            get
            {
                return Singleton<IdHelper>.Instance
                       ?? (Singleton<IdHelper>.Instance = new IdHelper());
            }
        }

        public Guid Guid
        {
            get { return GetGuid(); }
        }

        public string Guid32
        {
            get { return GetGuid32(); }
        }

        public long LongId
        {
            get { return GetLongId(); }
        }

        public int IntId
        {
            get { return GetIntId(); }
        }

        #region 生成GUID
        /// <summary>
        /// 生成标准GUID（例如：e85f942e-156d-47bc-a6c8-123a727a3a3a）
        /// </summary>
        /// <returns>Guid</returns>
        public Guid GetGuid()
        {
            return Guid.NewGuid();
        }

        /// <summary>
        /// 生成32位GUID（例如：e85f942e156d47bca6c8123a727a3a3a）
        /// </summary>
        /// <returns>Guid</returns>
        public string GetGuid32()
        {
            return GetGuid().ToString("N");
        }
        #endregion

        #region 生成数字型ID
        // 批量生成ID的时间刻度
        private const long TimeScaleLong = 100000000;
        // 批量生成ID的时间刻度
        private const long TimeScaleInt = 1000000000;
        // 生成ID的随机数长度
        private const int RandomLengthLong = 5;
        // 生成ID的随机数长度
        private const int RandomLengthInt = 3;
        // 计算时间间隔的开始时间
        private static readonly DateTime BeginDatetime = new DateTime(2013, 10, 1);
        // 随机数缓存
        private static readonly Dictionary<long, long> DictLong = new Dictionary<long, long>();
        // 随机数缓存
        private static readonly Dictionary<int, int> DictInt = new Dictionary<int, int>();
        // 时间戳缓存（上一次计算ID的系统时间按时间戳刻度取值）
        private static long _lastEndDatetimeTicksLong;
        // 时间戳缓存（上一次计算ID的系统时间按时间戳刻度取值）
        private static int _lastEndDatetimeTicksInt;
        // 静态随机数生成器
        private static Random _random;

        #region Long
        /// <summary>
        /// 生成Long型的数字ID
        /// </summary>
        /// <returns>Long型数字</returns>
        public long GetLongId()
        {
            var timeStamp = (DateTime.Now.Ticks - BeginDatetime.Ticks) / TimeScaleLong;
            if (timeStamp != _lastEndDatetimeTicksLong)
            {
                DictLong.Clear();
            }

            var power = long.Parse(Math.Pow(10, RandomLengthLong).ToString(CultureInfo.InvariantCulture));
            var rand = GetRandomLong(RandomLengthLong);
            var result = timeStamp * power + rand;

            if (DictLong.ContainsKey(result))
            {
                var isRepeat = true;

                for (var i = 0; i < power; i++)
                {
                    rand = GetRandomLong(RandomLengthLong);
                    result = timeStamp * power + rand;

                    if (!DictLong.ContainsKey(result))
                    {
                        DictLong.Add(result, result);
                        isRepeat = false;
                        break;
                    }
                }

                if (isRepeat)
                {
                    return 0L;
                }
            }
            else
            {
                DictLong.Add(result, result);
            }

            _lastEndDatetimeTicksLong = timeStamp;
            return result;
        }
        #endregion

        #region Int
        /// <summary>
        /// 生成Int型的数字ID
        /// </summary>
        /// <returns>Int型数字</returns>
        public int GetIntId()
        {
            var timeStamp = (int)((DateTime.Now.Ticks - BeginDatetime.Ticks) / TimeScaleInt);
            if (timeStamp != _lastEndDatetimeTicksInt)
            {
                DictInt.Clear();
            }

            var power = int.Parse(Math.Pow(10, RandomLengthInt).ToString(CultureInfo.InvariantCulture));
            var rand = GetRandomLong(RandomLengthInt);
            var result = (int)(timeStamp * power + rand);

            if (DictInt.ContainsKey(result))
            {
                var isRepeat = true;

                for (var i = 0; i < power; i++)
                {
                    rand = GetRandomLong(RandomLengthInt);
                    result = (int)(timeStamp * power + rand);

                    if (!DictInt.ContainsKey(result))
                    {
                        DictInt.Add(result, result);
                        isRepeat = false;
                        break;
                    }
                }

                if (isRepeat)
                {
                    return 0;
                }
            }
            else
            {
                DictInt.Add(result, result);
            }

            _lastEndDatetimeTicksInt = timeStamp;
            return result;
        }
        #endregion

        // 获取随机数
        private long GetRandomLong(int length)
        {
            if (_random == null)
                _random = RandomHelper.Random();

            const int min = 0;
            var max = int.Parse(Math.Pow(10, length).ToString(CultureInfo.InvariantCulture));
            return long.Parse(_random.Next(min, max).ToString(CultureInfo.InvariantCulture));
        }

        #endregion
    }
}
