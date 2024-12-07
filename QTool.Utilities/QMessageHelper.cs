using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTool
{
    public static class QMessageHelper
    {
        private const int Duration = 5000;
        private static Action<string, int> _showMessage;
        public static void Show(string content, int duration = Duration)
        {
            if (_showMessage == null) throw new QException("请调用 QMessageHelper.Init()");
            _showMessage(content, duration);
        }

        public static void Show(Exception exception, int duration = Duration)
        {
            Show(exception.GetInnerException().Message, duration);
        }

        public static void Init(Action<string, int> showMessage)
        {
            _showMessage = showMessage ?? throw new ArgumentNullException(nameof(showMessage));
        }
    }
}
