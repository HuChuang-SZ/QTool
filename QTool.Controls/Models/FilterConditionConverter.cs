using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTool.Controls.Models
{
    public class FilterConditionConverter : TypeConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                var str = value as string;
                if (string.IsNullOrWhiteSpace(str))
                {
                    throw new ArgumentException(nameof(value));
                }

                var datas = str.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (datas.Length == 0 || string.IsNullOrWhiteSpace(datas[0]))
                {
                    throw new ArgumentException($"“PropertyName”不能为空 或 空字符串。", nameof(value));
                }
                string propertyName = datas[0];

                FilterType type = FilterType.Object;
                if (datas.Length >= 2)
                {
                    if (!TryGetFilterType(datas[1], out FilterType filterType))
                        throw new ArgumentException($"无效的“FilterType”。", nameof(value));
                    else
                        type = filterType;
                }

                int? range = null;
                if (datas.Length == 3)
                {
                    if (!int.TryParse(datas[2], out int val))
                    {
                        throw new ArgumentException($"无效的“FilterRange”。", nameof(value));
                    }
                    else
                    {
                        range = val;
                    }
                }

                return new FilterCondition(propertyName, type, range);
            }

            throw new ArgumentException($"无效的格式转换。", nameof(value));
        }

        private bool TryGetFilterType(string inputVal, out FilterType type)
        {
            switch (inputVal.ToUpper())
            {
                case "N":
                    type = FilterType.Number;
                    return true;
                case "A":
                    type = FilterType.Array;
                    return true;
                case "O":
                    type = FilterType.Object;
                    return true;
                //case "H":
                //    type = FilterType.Hour;
                //    return true;
                //case "D":
                //    type = FilterType.Day;
                //    return true;
                //case "M":
                //    type = FilterType.Month;
                //    return true;
                //case "Y":
                //    type = FilterType.Year;
                //    return true;
                default:
                    break;
            }


            type = FilterType.Object;
            return false;
        }
    }
}
