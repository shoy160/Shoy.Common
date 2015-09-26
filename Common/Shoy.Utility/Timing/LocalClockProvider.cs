using System;

namespace Shoy.Utility.Timing
{
    public class LocalClockProvider : IClockProvider
    {
        public DateTime Now
        {
            get { return DateTime.Now; }
        }

        public DateTime Normalize(DateTime dateTime)
        {
            if (dateTime.Kind == DateTimeKind.Unspecified)
                return DateTime.SpecifyKind(dateTime, DateTimeKind.Local);
            return dateTime.Kind == DateTimeKind.Utc
                ? dateTime.ToLocalTime()
                : dateTime;
        }
    }
}
