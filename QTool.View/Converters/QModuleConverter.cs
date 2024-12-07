using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace QTool.View.Converters
{
    public class QModuleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return "全部权限";
            }
            else if (value is byte[] modules && modules?.Length > 0)
            {
                return string.Join("、", modules.Select(m => ((QModule)m).ToDisplayName()));
            }
            return "无权限";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
