
using System;

namespace Shoy.Utility.Timing
{
    public static class Clock
    {
        private static IClockProvider _clockProvider;

        public static IClockProvider ClockProvider
        {
            get { return _clockProvider; }
            set
            {
                if (value == null) return;
                _clockProvider = value;
            }
        }

        static Clock()
        {
            _clockProvider = new LocalClockProvider();
        }

        public static DateTime Now
        {
            get { return _clockProvider.Now; }
        }

        public static DateTime Normalize(DateTime dateTime)
        {
            return _clockProvider.Normalize(dateTime);
        }

        public static DateTime ZoneTime = new DateTime(1970, 1, 1);

        public static DateTime ToDateTime(this long time)
        {
            return time <= 0L ? ZoneTime : new DateTime(ZoneTime.Ticks + time * 10000L);
        }

        public static long ToLong(this DateTime time)
        {
            if (time <= ZoneTime)
                return 0L;
            return (time.Ticks - ZoneTime.Ticks) / 10000L;
        }
    }
}
