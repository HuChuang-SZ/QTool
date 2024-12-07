using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTool
{
    public static class TimeZones
    {
        /// <summary>
        /// 北京时间
        /// </summary>
        public static TimeZoneInfo ChinaStandardTime { get; } = TimeZoneInfo.FindSystemTimeZoneById("China Standard Time");

        /// <summary>
        /// 俄罗斯时间
        /// </summary>
        public static TimeZoneInfo RussianStandardTime { get; } = TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time");

        /// <summary>
        /// 太平洋时间
        /// </summary>
        public static TimeZoneInfo PacificStandardTime { get; } = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
    }
}
