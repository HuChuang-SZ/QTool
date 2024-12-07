using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTool
{
    public static class DisplayHelper
    {
        public static string GetDiscountPriceStr(string currencyCode, int discount, decimal minPrice, decimal maxPrice)
        {
            return GetPriceStr(currencyCode, GetDiscountPrice(minPrice, discount), GetDiscountPrice(maxPrice, discount));
        }

        public static string GetDiscountStr(int discount)
        {
            return $"{discount}% off";
        }

        public static string GetPriceStr(string currencyCode, decimal minPrice, decimal maxPrice)
        {
            if (minPrice != maxPrice)
            {
                return $"{currencyCode} {minPrice:0.00} - {maxPrice:0.00}";
            }
            else
            {
                return $"{currencyCode} {minPrice:0.00}";
            }
        }

        public static decimal GetDiscountPrice(decimal originPrice, int discount)
        {
            return Math.Round(originPrice * (100 - discount) / 100, 2);
        }

        public static string GetRegionName(this string regionCode)
        {
            switch (regionCode)
            {
                case "ES": return "西班牙";
                case "FR": return "法国";
                case "BR": return "巴西";
                case "US": return "美国";
                case "KR": return "韩国";
                case "RU": return "俄罗斯";
                case "JV": return "俄语系国家(除俄罗斯)";
                case "GCC": return "中东6国";
                case "OTHERS": return "其他国家";
                case "ALL": return "全站";
                default:
                    return regionCode;
            }
        }
    }
}
