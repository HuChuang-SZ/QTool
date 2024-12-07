using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTool.Controls.Models
{
    public class FilterNumberValue : IComparable, IComparable<FilterNumberValue>
    {
        public FilterNumberValue(object value, int range)
        {
            var val = value.ToDecimal();
            if (val == null)
            {
                MinValue = null;
                MaxValue = null;
            }
            else
            {
                MinValue = (int)(val.Value / range) * range;
                MaxValue = MinValue + range;
            }
        }

        public int CompareTo(object other)
        {
            if (other == null)
            {
                return 1;
            }

            if (other is FilterNumberValue)
            {
                return CompareTo((FilterNumberValue)other);
            }
            else
            {
                throw new ArgumentException("“other”参数无法转换为“Int32Limit”类型");
            }
        }

        public int CompareTo(FilterNumberValue other)
        {
            if (IsEmpty)
            {
                if (other.IsEmpty)
                {
                    return 0;
                }
                else
                {
                    return 1;
                }
            }


            if (!CompareHelper.TryCompareTo(MaxValue, other.MinValue, out int compareVal))
            {
                return compareVal;
            }

            if (!CompareHelper.TryCompareTo(MaxValue, other.MaxValue, out compareVal))
            {
                return compareVal;
            }

            return 0;
        }

        public decimal? MinValue { get; }

        public decimal? MaxValue { get; }

        public bool IsEmpty { get { return !MinValue.HasValue && !MaxValue.HasValue; } }

        public bool ValueCompareTo(object value)
        {
            var d = value.ToDecimal();
            if (IsEmpty)
            {
                return d == null;
            }
            else
            {
                if (d.HasValue)
                {
                    if (MinValue.HasValue && MinValue.Value > d)
                    {
                        return false;
                    }

                    if (MaxValue.HasValue && MaxValue.Value <= d)
                    {
                        return false;
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public override string ToString()
        {
            if (MinValue.HasValue)
            {
                if (MaxValue.HasValue)
                {
                    return $"{MinValue.Value:0.##} - {MaxValue.Value:0.##}";
                }
                else
                {
                    return $"{MinValue.Value:0.##}+";
                }
            }
            else
            {
                if (MaxValue.HasValue)
                {
                    return $"{MaxValue.Value:0.##}-";
                }
                else
                {
                    return "无";
                }
            }
        }
    }
}
