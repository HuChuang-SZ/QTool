using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace QTool.Controls.Converters
{
    public class TimeSpanToSecondsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType == typeof(double) && value is TimeSpan)
            {
                TimeSpan timeSpan = (TimeSpan)value;
                return timeSpan.TotalSeconds;
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType == typeof(TimeSpan) && value is double)
            {
                var seconds = (double)value;
                return TimeSpan.FromSeconds(seconds);
            }

            return value;
        }
    }
}
