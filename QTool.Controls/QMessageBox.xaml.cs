
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace QTool.Controls
{
    /// <summary>
    /// QMessageBox.xaml 的交互逻辑
    /// </summary>
    public partial class QMessageBox : Window
    {
        private readonly Window _owner;
        private QMessageBox(Window owner, string content, int duration)
        {
            InitializeComponent();
            _owner = Owner = owner;
            txtContent.Inlines.AddRange(content.ToInlines());
            if (duration > 0)
            {
                ExpirationTime = DateTime.Now.AddMilliseconds(duration);
                var timer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromMilliseconds(20)
                };
                timer.Tick += Timer_Tick;
                timer.Start();
            }

            Loaded += QMessageBox_Loaded;
        }

        private const int TransparentMilliseconds = 2000;
        private const int Duration = 5000;

        private void Timer_Tick(object sender, EventArgs e)
        {
            var totalMilliseconds = (int)(ExpirationTime.Value - DateTime.Now).TotalMilliseconds;
            if (totalMilliseconds > TransparentMilliseconds)
            {
                Opacity = 1;
            }
            else if (IsMouseOver)
            {
                ExpirationTime = DateTime.Now.AddMilliseconds(TransparentMilliseconds);
                Opacity = 1;
            }
            else
            {
                Opacity = 1d * totalMilliseconds / TransparentMilliseconds;
            }
        }

        private void QMessageBox_Loaded(object sender, RoutedEventArgs e)
        {
            InitPosition();
            _windows.Add(this);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _windows.Remove(this);
        }

        private void InitPosition()
        {
            Rect rect;

            if (_owner?.Visibility == Visibility.Visible)
            {
                if (_owner.WindowState == WindowState.Normal)
                {
                    rect = new Rect(_owner.Left, _owner.Top, _owner.ActualWidth, _owner.ActualHeight);
                }
                else
                {
                    rect = new Rect(0, 0, _owner.ActualWidth, _owner.ActualHeight);
                }
            }
            else
            {
                rect = SystemParameters.WorkArea;
            }

            var prevWindow = _windows.LastOrDefault(w => w._owner == _owner);
            if (prevWindow != null)
            {
                Top = prevWindow.Top + prevWindow.ActualHeight + 10;
            }
            else
            {
                Top = rect.Y + 50;
            }
            Left = rect.X + (rect.Width - ActualWidth) / 2;
        }

        public DateTime? ExpirationTime { get; private set; }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private static List<QMessageBox> _windows = new List<QMessageBox>();
        private readonly static DispatcherTimer _globalTimer;
        static QMessageBox()
        {
            _globalTimer = new DispatcherTimer();
            _globalTimer.Interval = TimeSpan.FromMilliseconds(100);
            _globalTimer.Tick += GlobalTimer_Tick;
            _globalTimer.Start();
        }

        private static void GlobalTimer_Tick(object sender, EventArgs e)
        {
            var nowTime = DateTime.Now;
            var windows = _windows.ToArray();
            foreach (var window in windows)
            {
                if (window.ExpirationTime.HasValue)
                {
                    if (window.ExpirationTime < nowTime)
                    {
                        window.Close();
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <param name="owner"></param>
        /// <param name="duration">显示时长(ms),默认值：5000ms，0表示不自动关闭</param>
        /// <exception cref="ArgumentException"></exception>
        public static void Show(string content, Window owner, int duration = Duration)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                throw new ArgumentException($"“{nameof(content)}”不能为 null 或空白。", nameof(content));
            }

            if (Application.Current != null)
            {
                if (Application.Current.CheckAccess())
                {
                    if (owner == null)
                    {
                        owner = AppHelper.GetActiveWindow();
                    }
                    QMessageBox messageBox = new QMessageBox(owner, content, duration);
                    messageBox.Show();
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Show(content, owner, duration);
                    });
                }
            }
        }

        public static void Show(Exception exception, Window owner = null, int duration = Duration)
        {
            Show(exception.GetInnerException().Message, owner, duration);
        }

        public static void Show(string content, int duration = Duration)
        {
            Show(content, null, duration);
        }
    }
}
