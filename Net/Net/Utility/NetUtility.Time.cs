using System;
using System.Net;

namespace Net
{
    public partial class NetUtility
    {
        public class Time
        {
            private static readonly DateTime epoch = new DateTime(1970, 1, 1);
            private static readonly DateTime twepoch = new DateTime(2000, 1, 1);

            public static UInt32 GetIclock()
            {
                var now = Convert.ToInt64(DateTime.Now.Subtract(twepoch).TotalMilliseconds);
                return (UInt32)(now & 0xFFFFFFFF);
            }

            public static Int64 LocalUnixTime()
            {
                return Convert.ToInt64(DateTime.Now.Subtract(epoch).TotalMilliseconds);
            }

            // local datetime to unix timestamp
            public static Int64 ToUnixTimestamp(DateTime t)
            {
                var timespan = t.ToUniversalTime().Subtract(epoch);
                return (Int64)Math.Truncate(timespan.TotalSeconds);
            }
        }
    }
}
