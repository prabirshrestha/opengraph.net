using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Facebook.Graph.Util
{
    internal static class UnixTime
    {
        private static readonly DateTime UnixEpochStartUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private static readonly DateTime UnixEpochStartLocal = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local);

        public static long FromDateTime(DateTime dt)
        {
            TimeSpan ts = dt - UnixEpochStartLocal;
            return (long)ts.TotalSeconds;
        }

        public static DateTime ToDateTime(long seconds)
        {
            return UnixEpochStartLocal.AddSeconds(seconds);
        }
    }
}
