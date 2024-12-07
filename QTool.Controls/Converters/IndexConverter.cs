using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace QTool.Controls.Converters
{
    public class IndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (TryGetInt32(value, out int val))
            {
                return val + 1;
            }
            else
            {
                return value;
            }
        }

        private bool TryGetInt32(object value, out int val)
        {
            if (value != null)
                return int.TryParse(value.ToString(), out val);
            else
            {
                val = 0;
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (TryGetInt32(value, out int val))
            {
                return val - 1;
            }
            else
            {
                return value;
            }
        }
    }
}
