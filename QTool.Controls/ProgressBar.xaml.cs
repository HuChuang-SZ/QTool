using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace QTool.Controls
{
    /// <summary>
    /// ProgressBar.xaml 的交互逻辑
    /// </summary>
    public partial class ProgressBar : UserControl
    {
        public ProgressBar()
        {
            InitializeComponent();
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            InitProgress();
        }

        public bool IsShow
        {
            get { return (bool)GetValue(IsShowProperty); }
            set { SetValue(IsShowProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsShow.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsShowProperty =
            DependencyProperty.Register("IsShow", typeof(bool), typeof(ProgressBar), new UIPropertyMetadata(false, OnIsShowChanged));
        private static void OnIsShowChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ProgressBar bar = sender as ProgressBar;
            if (bar != null)
            {
                if ((bool)e.NewValue)
                {
                    bar.Visibility = Visibility.Visible;
                }
                else
                {
                    bar.Visibility = Visibility.Collapsed;
                }
            }
        }


        public bool IsCoverUp
        {
            get { return (bool)GetValue(IsCoverUpProperty); }
            set { SetValue(IsCoverUpProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsCoverUp.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsCoverUpProperty =
            DependencyProperty.Register("IsCoverUp", typeof(bool), typeof(ProgressBar), new UIPropertyMetadata(true, OnIsCoverUpChanged));
        private static void OnIsCoverUpChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ProgressBar bar = sender as ProgressBar;
            if (bar != null)
            {
                if ((bool)e.NewValue)
                {
                    bar.coverUp.Visibility = Visibility.Visible;
                }
                else
                {
                    bar.coverUp.Visibility = Visibility.Collapsed;
                }
            }
        }



        public bool IsShowProgress
        {
            get { return (bool)GetValue(IsShowProgressProperty); }
            set { SetValue(IsShowProgressProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsShowProgress.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsShowProgressProperty =
            DependencyProperty.Register("IsShowProgress", typeof(bool), typeof(ProgressBar), new UIPropertyMetadata(false, OnIsShowProgressChanged));
        private static void OnIsShowProgressChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ProgressBar bar = sender as ProgressBar;
            if (bar != null)
            {
                if ((bool)e.NewValue)
                {
                    bar.progressText.Visibility = Visibility.Visible;
                    bar.CalcProgressPct();
                }
                else
                {
                    bar.progressText.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void CalcProgressPct()
        {
            if (MaximunProgress > 0)
            {
                progressText.Text = string.Format("{0:0}", Progress / MaximunProgress * 100);
            }
            else
            {
                progressText.Text = "";
            }
        }


        public double Progress
        {
            get { return (double)GetValue(ProgressProperty); }
            set { SetValue(ProgressProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Progress.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ProgressProperty =
            DependencyProperty.Register("Progress", typeof(double), typeof(ProgressBar), new UIPropertyMetadata(0d, OnProgressChanged));
        private static void OnProgressChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ProgressBar bar = sender as ProgressBar;
            if (bar != null)
            {
                bar.CalcProgressPct();
            }
        }


        public double MaximunProgress
        {
            get { return (double)GetValue(MaximunProgressProperty); }
            set { SetValue(MaximunProgressProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaximunProgress.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaximunProgressProperty =
            DependencyProperty.Register("MaximunProgress", typeof(double), typeof(ProgressBar), new UIPropertyMetadata(100d, OnMaximunProgressChanged));
        private static void OnMaximunProgressChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ProgressBar bar = sender as ProgressBar;
            if (bar != null)
            {
                bar.CalcProgressPct();
            }
        }


        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Message.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(string), typeof(ProgressBar), new UIPropertyMetadata(null, OnMessageChanged));

        private static void OnMessageChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ProgressBar bar = sender as ProgressBar;
            if (bar != null)
            {
                var msg = e.NewValue as string;
                if (string.IsNullOrWhiteSpace(msg))
                {
                    bar.message.Text = string.Empty;
                    bar.message.Visibility = Visibility.Collapsed;
                }
                else
                {
                    bar.message.Text = msg;
                    bar.message.Visibility = Visibility.Visible;
                }
            }
        }


        public int MaxDiameter
        {
            get { return (int)GetValue(MaxDiameterProperty); }
            set { SetValue(MaxDiameterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaxDiameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxDiameterProperty =
            DependencyProperty.Register("MaxDiameter", typeof(int), typeof(ProgressBar), new PropertyMetadata(60));

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            InitProgress();
            base.OnRenderSizeChanged(sizeInfo);
        }

        private double CalcDiameter()
        {
            double minValue = Math.Min(ActualHeight, ActualWidth);
            if (!string.IsNullOrEmpty(Message))
            {
                minValue -= 20;
            }
            if (minValue > MaxDiameter)
                return MaxDiameter;
            else
                return minValue;
        }

        protected void InitProgress()
        {
            if (IsInitialized)
            {
                var diameter = CalcDiameter();

                double circumference = diameter * Math.PI;
                double dotCount, dotDiameter;
                if (diameter > MaxDiameter)
                {
                    dotCount = 15;
                    progressText.FontSize = 18;
                }
                else if (diameter > 50)
                {
                    dotCount = 14;
                    progressText.FontSize = 16;
                }
                else if (diameter > 40)
                {
                    dotCount = 13;
                    progressText.FontSize = 14;
                }
                else if (diameter > 30)
                {
                    dotCount = 12;
                    progressText.FontSize = 12;
                }
                else if (diameter > 20)
                {
                    dotCount = 11;
                    progressText.FontSize = 8;
                }
                else
                {
                    dotCount = 10;
                    progressText.FontSize = 6;
                }
                dotDiameter = circumference / dotCount / 1.8;

                double Angle = 360.0 / dotCount;
                double Radii = (diameter - dotDiameter) / 2;
                progress.Children.Clear();
                progress.Width = progress.Height = diameter;
                progress.Visibility = Visibility.Visible;
                for (int i = 0; i < dotCount; i++)
                {
                    Ellipse ellipse = new Ellipse();
                    ellipse.Height = ellipse.Width = dotDiameter;
                    ellipse.Opacity = 1.0 / dotCount * i;
                    Canvas.SetLeft(ellipse, Radii * Math.Cos(Math.PI * (i * Angle) / 180) + Radii);
                    Canvas.SetTop(ellipse, Radii * Math.Sin(Math.PI * (i * Angle) / 180) + Radii);
                    progress.Children.Add(ellipse);
                }
            }
        }
    }
}
