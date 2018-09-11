using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMP.Generic_Functions
{
    public class LocalTimeZone
    {
        public static DateTime GetCurrentTime()
        {
            DateTime serverTime = DateTime.Now;
            DateTime _localTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(serverTime, TimeZoneInfo.Local.Id, "Pakistan Standard Time");
            return _localTime;
        }
    }
}