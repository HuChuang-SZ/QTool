using CefSharp;
using QTool.Controls;
using QTool.View.Contents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace QTool.View.Controls
{
    /// <summary>
    /// ProductAnalyseControl.xaml 的交互逻辑
    /// </summary>
    public partial class AeBrowserControl : UserControl
    {
        public AeBrowserControl()
        {
            InitializeComponent();
        }

        private void Send_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (Content is AeBrowserContent content && content.Content?.Browser != null && Uri.TryCreate(content.Content.Browser.Address, UriKind.Absolute, out Uri uri) && string.Compare(uri.LocalPath, "/apps/csp/im", true) == 0)
            {
                if (!content.Content.Browser.IsDisposed)
                {
                    var frame = content.Content.Browser.GetMainFrame();
                    if (frame.IsValid && frame.IsFocused)
                    {
                        var result = frame.EvaluateScriptAsync("document.getElementsByTagName('a').forEach(function(elem){ if(elem.ariaDisabled) elem.click(); });").Result;
                    }
                }
            }
        }
    }
}