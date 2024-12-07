
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace QTool.Controls.Converters
{
    public class NumberToBooleanConverter : IValueConverter
    {
        public ComparisonMode Mode { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = value.ToDouble();
            var para = parameter.ToDouble();
            if (val.HasValue && para.HasValue)
            {
                switch (Mode)
                {
                    case ComparisonMode.GreaterThan:
                        return val > para;

                    case ComparisonMode.LessThan:
                        return val < para;

                    case ComparisonMode.EqualTo:
                        return val == para;
                }
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public enum ComparisonMode
    {
        GreaterThan,
        EqualTo,
        LessThan
    }
}
