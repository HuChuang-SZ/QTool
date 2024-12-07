using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTool
{
    public static class CompareHelper
    {

        public static bool TryCompareTo<T>(T? valueA, T? valueB, out int compareVal)
            where T : struct, IComparable<T>
        {
            compareVal = CompareTo(valueA, valueB);
            return compareVal == 0;
        }

        public static int CompareTo<T>(T? valueA, T? valueB)
            where T : struct, IComparable<T>
        {
            if (valueA.HasValue)
            {
                if (valueB.HasValue)
                {
                    return valueA.Value.CompareTo(valueB.Value);
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                if (valueB.HasValue)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }

        public static int CompareTo(object valueA, object valueB)
        {
            if (valueA != null)
            {
                if (valueB != null)
                {
                    return CompareTo(valueA as IComparable, valueB as IComparable);
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                if (valueB != null)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }

        public static int CompareTo(IComparable valueA, IComparable valueB)
        {
            if (valueA != null)
            {
                if (valueB != null)
                {
                    return valueA.CompareTo(valueB);
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                if (valueB != null)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }

        public static bool TryCompareTo(object valueA, object valueB, out int compareVal)
        {
            compareVal = CompareTo(valueA, valueB);
            return compareVal == 0;
        }
    }
}
