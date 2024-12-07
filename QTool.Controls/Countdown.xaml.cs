using NPOI.HSSF.Record.Chart;
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
using System.Windows.Threading;

namespace QTool.Controls
{
    /// <summary>
    /// Countdown.xaml 的交互逻辑
    /// </summary>
    public partial class Countdown : UserControl
    {
        private readonly DispatcherTimer _timer;
        public Countdown()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;
            InitializeComponent();

        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            RefreshView();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            RefreshView();
        }

        private void RefreshView()
        {
            if (IsInitialized)
            {
                var expirationTime = ExpirationTime ?? default;
                if (textBlock.Inlines.Count > 0)
                    textBlock.Inlines.Clear();
                DateTime now = DateTime.Now;
                if (expirationTime > now)
                {
                    TimeSpan timeSpan = expirationTime - now;
                    string title = Title;
                    if (!string.IsNullOrWhiteSpace(title))
                    {
                        textBlock.Inlines.Add(title);
                        textBlock.Inlines.Add(new LineBreak());
                    }

                    if (timeSpan.TotalDays > 1)
                    {
                        _timer.Interval = TimeSpan.FromMinutes(1);
                        //timeSpanFormat = "d'天'hh'时'";
                        var foreground = FindResource("color-warning") as SolidColorBrush;

                        textBlock.Inlines.AddRange(CreateRun(timeSpan.Days, "天", foreground));
                        textBlock.Inlines.AddRange(CreateRun(timeSpan.Hours, "时", foreground));
                    }
                    else
                    {
                        _timer.Interval = TimeSpan.FromSeconds(1);
                        var foreground = FindResource("color-danger") as SolidColorBrush;
                        textBlock.Inlines.AddRange(CreateRun(timeSpan.Hours, "时", foreground));
                        textBlock.Inlines.AddRange(CreateRun(timeSpan.Minutes, "分", foreground));
                        textBlock.Inlines.AddRange(CreateRun(timeSpan.Seconds, "秒", foreground));
                    }
                    TimerStart();
                }
                else
                {
                    textBlock.Inlines.Add("-");
                    TimerStop();
                }
            }
        }

        private IEnumerable<Run> CreateRun(int value, string unit, SolidColorBrush foreground)
        {
            yield return new Run(value.ToString("00")) { FontSize = 16, Foreground = foreground };
            yield return new Run(string.Concat(' ', unit, ' '));
        }

        private void TimerStop()
        {
            if (_timer.IsEnabled)
                _timer.Stop();
        }

        private void TimerStart()
        {
            if (!_timer.IsEnabled)
            {
                _timer.Start();
            }
        }



        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(Countdown), new PropertyMetadata(null, OnTitleChanged));

        private static void OnTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Countdown)d).RefreshView();
        }

        public DateTime? ExpirationTime
        {
            get { return (DateTime?)GetValue(ExpirationTimeProperty); }
            set { SetValue(ExpirationTimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ExpirationTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ExpirationTimeProperty =
            DependencyProperty.Register("ExpirationTime", typeof(DateTime?), typeof(Countdown), new PropertyMetadata(null, OnExpirationTimeChanged));

        private static void OnExpirationTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Countdown)d).OnExpirationTimeChanged(e.NewValue as DateTime?, e.OldValue as DateTime?);
        }

        private void OnExpirationTimeChanged(DateTime? newValue, DateTime? oldValue)
        {
            RefreshView();
        }

    }
}
