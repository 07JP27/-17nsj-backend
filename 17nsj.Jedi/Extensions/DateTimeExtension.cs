using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _17nsj.Jedi.Extensions
{
    public static class DateTimeExtension
    {
        public static DateTime TruncMillSecond(this DateTime source)
        {
            var d = new DateTime(source.Year, source.Month, source.Day, source.Hour, source.Minute, source.Second, 0);
            return d;
        }
    }
}
