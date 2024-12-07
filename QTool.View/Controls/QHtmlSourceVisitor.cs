using CefSharp;
using CefSharp.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTool.View.Controls
{
    public class QHtmlSourceVisitor : IStringVisitor
    {
        private readonly Action<string> _callback;

        public QHtmlSourceVisitor(Action<string> callback)
        {
            _callback = callback ?? throw new ArgumentNullException(nameof(callback));
        }

        public void Dispose()
        {

        }

        public void Visit(string htmlString)
        {
            _callback.Invoke(htmlString);
        }
    }
}
