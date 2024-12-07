using QTool.Controls;
using QTool.Controls.Utilities;
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
using System.Windows.Shapes;

namespace QTool.View
{
    /// <summary>
    /// VideoWindow.xaml 的交互逻辑
    /// </summary>
    public partial class VideoView : QWindow
    {
        public VideoView()
        {
            InitializeComponent();
        }

        public static void ShowDialog(string uriString, DependencyObject owner)
        {
            if (Application.Current?.CheckAccess() == false)
            {
                Application.Current.Dispatcher.Invoke(new Action<string, DependencyObject>(ShowDialog), new object[] { uriString, owner });
            }
            else
            {
                if (ImageLoader.TryCreateUri(uriString, out Uri uri))
                {
                    var dialog = new VideoView();
                    dialog.Owner = GetWindow(owner);
                    dialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    dialog.Player.Source = uri;
                    dialog.ShowDialog();
                }
                else
                {
                    QMessageBox.Show($"无效的视频链接（{uriString}）。", GetWindow(owner));
                }
            }
        }
    }
}
