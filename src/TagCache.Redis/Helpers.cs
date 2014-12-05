using System;
using System.Collections.Generic;

namespace TagCache.Redis
{
    internal static class Helpers
    {

        internal static KeyValuePair<string, DateTime> ToKeyValuePairStringDate(KeyValuePair<byte[], double> x)
        {
            return new KeyValuePair<string, DateTime>(BytesToString(x.Key), RankToTime(x.Value));
        }

        internal static string BytesToString(byte[] arr)
        {
            return System.Text.Encoding.UTF8.GetString(arr);
        }


        internal static KeyValuePair<string, DateTime> ToKeyValuePairStringDate(KeyValuePair<string, double> x)
        {
            return new KeyValuePair<string, DateTime>(x.Key, RankToTime(x.Value));
        }

        internal static DateTime RankToTime(double value)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(value);
        }

        internal static long TimeToRank(DateTime date)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToInt64((date - epoch).TotalSeconds);
        }

    }
}
