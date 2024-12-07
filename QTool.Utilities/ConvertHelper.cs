using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QTool
{
    public static class ConvertHelper
    {
        public static double? ToDouble(this object inputObj)
        {
            if (inputObj != null && double.TryParse(inputObj.ToString().Trim(), out double val))
            {
                return val;
            }

            return null;
        }


        public static int? ToInt32(this object inputObj)
        {
            var inputStr = inputObj?.ToString();
            if (!string.IsNullOrEmpty(inputStr) && int.TryParse(inputStr.Trim(), out int val))
                return val;
            else
                return null;
        }

        public static decimal? ToDecimal(this object inputObj)
        {
            if (inputObj != null && decimal.TryParse(inputObj.ToString().Trim(), out decimal val))
            {
                return val;
            }
            return null;
        }

        public static decimal? ToPercent(this string inputStr)
        {
            if (!string.IsNullOrWhiteSpace(inputStr))
            {
                if (inputStr.Last() == '%')
                {
                    if (decimal.TryParse(inputStr.Substring(0, inputStr.Length - 1), out decimal val))
                    {
                        return val / 100;
                    }
                }
            }
            return null;
        }

        public static decimal ToPercentValue(this string inputStr)
        {
            return inputStr.ToPercent() ?? throw new ArgumentException($"无法将“{inputStr}”值转换为百分比");
        }

        public static decimal? ToAmount(this string inputStr)
        {
            if (!string.IsNullOrWhiteSpace(inputStr))
            {
                if (decimal.TryParse(inputStr.Trim(' ', '$', '￥'), out decimal val))
                {
                    return val;
                }
            }
            return null;
        }

        public static DateTime? ToDateTime(this object inputObj)
        {
            if (inputObj != null && DateTime.TryParse(inputObj.ToString().Trim(), out DateTime val))
            {
                return val;
            }

            return null;
        }

        public static long? ToInt64(this object inputObj)
        {
            if (inputObj != null && long.TryParse(inputObj.ToString().Trim(), out long val))
            {
                return val;
            }

            return null;
        }

        public static bool? ToBoolean(this object inputObj)
        {
            if (inputObj != null)
            {
                return inputObj.ToString().Trim().ToBoolean();
            }

            return null;
        }

        public static bool? ToBoolean(this string inputStr)
        {
            if (bool.TryParse(inputStr, out bool val))
            {
                return val;
            }

            return null;
        }

        public static bool ToBooleanValue(this string inputStr)
        {
            return inputStr.ToBoolean() ?? throw new ArgumentException($"无法将“{inputStr}”值转换为Boolean类型");
        }

        /// <summary>
        /// 获取时间戳(毫秒)
        /// </summary>
        /// <param name="date">时间戳</param>
        /// <returns></returns>
        public static long ToTimestamp(this DateTime date)
        {
            return (date.ToUniversalTime().Ticks - 621355968000000000) / 10000;
        }

        public static DateTime ToDateTime(this long timestamp)
        {
            return ToDateTime(timestamp, TimeZones.ChinaStandardTime);
        }
        public static DateTime ToDateTimeByUs(this long timestamp)
        {
            return ToDateTime(timestamp, TimeZones.PacificStandardTime);
        }

        public static DateTime ToDateTime(this long timestamp, TimeZoneInfo timeZone)
        {
            var beginTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            //             1687425620832 毫秒
            if (timestamp < 1000000000000)
            {
                return beginTime.AddSeconds(timestamp).ConvertTime(timeZone);
            }
            else
            {
                return beginTime.AddMilliseconds(timestamp).ConvertTime(timeZone);
            }
        }

        public static DateTime? AddMilliseconds(this DateTime dateTime, long? milliseconds)
        {
            if (milliseconds.HasValue)
            {
                return dateTime.AddMilliseconds(milliseconds.Value);
            }
            else
            {
                return null;
            }
        }

        public static DateTime ConvertTime(this DateTime dateTime, TimeZoneInfo timeZone)
        {
            return TimeZoneInfo.ConvertTime(dateTime, timeZone);
        }

        public static Dictionary<string, string> ToDictionary(this JObject jObject)
        {
            if (jObject != null && jObject.HasValues)
            {
                var datas = new Dictionary<string, string>();
                foreach (JProperty jProperty in jObject.Properties())
                {
                    datas.Add(jProperty.Name, jProperty.Value?.ToString());
                }

                return datas;
            }
            return null;
        }

        public static string ToFormat<T>(this T? value, string format)
            where T : struct, IFormattable
        {
            if (value.HasValue)
            {
                return value.Value.ToString(format, null);
            }
            else
            {
                return string.Empty;
            }
        }


        public static string ToKey(this string key, KeyOptions options = KeyOptions.IgnoreCase)
        {
            if (key == null)
            {
                return key;
            }
            else
            {
                var keyStr = key;
                if ((options & KeyOptions.IgnoreCase) == KeyOptions.IgnoreCase)
                {
                    keyStr = keyStr.ToLower();
                }

                if ((options & KeyOptions.IgnoreSpace) == KeyOptions.IgnoreSpace)
                {
                    keyStr = keyStr.Replace(" ", "").Replace("　", "").Replace("\t", "").Trim();
                }
                return keyStr;
            }
        }

        public static string ToSingleLine(this string inputStr)
        {
            if (inputStr == null)
            {
                return string.Empty;
            }
            else
            {
                return inputStr.Replace("\r", "\\r").Replace("\n", "\\n").Replace("\t", "\\t");
            }
        }

        public static TEnum? ToEnum<TEnum>(this string inputStr)
            where TEnum : struct
        {
            if (string.IsNullOrWhiteSpace(inputStr))
            {
                return null;
            }
            else if (Enum.TryParse(inputStr, true, out TEnum result))
            {
                return result;
            }
            else
            {
                throw new QException($"未知的枚举值：{typeof(TEnum).Name}.{inputStr}");
            }
        }

        public static TEnum ToEnumValue<TEnum>(this string inputStr)
            where TEnum : struct
        {
            if (string.IsNullOrWhiteSpace(inputStr))
            {
                throw new ArgumentNullException(inputStr);
            }

            if (Enum.TryParse(inputStr, true, out TEnum result))
            {
                return result;
            }
            else
            {
                throw new QException($"未知的枚举值：{typeof(TEnum).Name}.{inputStr}");
            }
        }

        public static string[] SplitToString(this string inputStr)
        {
            if (string.IsNullOrWhiteSpace(inputStr))
            {
                return Array.Empty<string>();
            }
            return inputStr.Split(new char[] { '\r', '\n', '\t', ' ', '　', ',', '，', ';', '；', '、' }, StringSplitOptions.RemoveEmptyEntries);
        }

        public static long[] SplitToInt64(this string inputStr)
        {
            string[] array = SplitToString(inputStr);
            List<long> result = new List<long>();
            foreach (var item in array)
            {
                if (long.TryParse(item, out long val))
                {
                    result.Add(val);
                }
            }
            return result.ToArray();
        }

        public static int[] SplitToInt32(this string inputStr)
        {
            string[] array = inputStr.SplitToString();
            List<int> result = new List<int>();
            foreach (var item in array)
            {
                if (int.TryParse(item, out int val))
                {
                    result.Add(val);
                }
            }

            return result.ToArray();
        }

        public static IPAddress ToIPAddress(this int val)
        {
            return new IPAddress(BitConverter.GetBytes(val));
        }

        public static int ToDate(this DateTime date)
        {
            return int.Parse(date.ToString("yyyyMMdd"));
        }

        public static string GetString(this TimeSpan timeSpan)
        {
            if (timeSpan.TotalDays >= 1)
            {
                return $"{timeSpan.Days}天{timeSpan.Hours:00}小时{timeSpan.Minutes:00}分钟";
            }
            else if (timeSpan.TotalHours >= 1)
            {
                return $"{timeSpan.Hours}小时{timeSpan.Minutes:00}分钟{timeSpan.Seconds:00}秒";
            }
            else if (timeSpan.TotalMinutes >= 1)
            {
                return $"{timeSpan.Minutes}分钟{timeSpan.Seconds:00}秒";
            }
            else
            {
                return $"{timeSpan.Seconds}秒";
            }
        }
    }
}
