using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace QTool.Controls.Converters
{
    public class TimeDifferenceConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values?.Length == 2 && values[0] is DateTime dateTimeA && values[1] is DateTime dateTimeB && dateTimeA > dateTimeB)
            {
                var timeSpan = dateTimeA - dateTimeB;
                return timeSpan.GetString();
            }
            else
            {
                return "-";
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
