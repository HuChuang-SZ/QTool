
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace QTool.Controls.Converters
{
    public class ProportionConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 2 && TryDecimal(values[0], out decimal d0) && TryDecimal(values[1], out decimal d1))
            {
                if (d1 != 0)
                    return d0 / d1;
                else
                    return 0d;
            }

            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public bool TryDecimal(object value, out decimal result)
        {
            if (value != null && decimal.TryParse(value.ToString(), out result))
            {
                return true;
            }
            result = default;
            return false;
        }
    }
}
