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

namespace QTool.Controls
{
    /// <summary>
    /// StatusControl.xaml 的交互逻辑
    /// </summary>
    public partial class StatusControl : UserControl
    {
        public StatusControl()
        {
            InitializeComponent();
        }



        public bool IsAbnormal
        {
            get { return (bool)GetValue(IsAbnormalProperty); }
            set { SetValue(IsAbnormalProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsAbnormal.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsAbnormalProperty =
            DependencyProperty.Register("IsAbnormal", typeof(bool), typeof(StatusControl), new PropertyMetadata(false, OnIsAbnormalChanged));

        private static void OnIsAbnormalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((StatusControl)d).UpdateStatus();
        }

        private void UpdateStatus()
        {
            if (IsAbnormal)
            {
                ContentTemplate = FindResource("AbnormalTemplate") as DataTemplate;
                Content = AbnormalText;
            }
            else
            {
                ContentTemplate = FindResource("NormalTemplate") as DataTemplate;
                Content = NormalText;
            }
        }

        public string NormalText
        {
            get { return (string)GetValue(NormalTextProperty); }
            set { SetValue(NormalTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NormalText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NormalTextProperty =
            DependencyProperty.Register("NormalText", typeof(string), typeof(StatusControl), new PropertyMetadata(null, OnNormalTextChanged));

        private static void OnNormalTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((StatusControl)d).UpdateStatus();
        }

        public string AbnormalText
        {
            get { return (string)GetValue(AbnormalTextProperty); }
            set { SetValue(AbnormalTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AbnormalText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AbnormalTextProperty =
            DependencyProperty.Register("AbnormalText", typeof(string), typeof(StatusControl), new PropertyMetadata(null, OnAbnormalTextChanged));

        private static void OnAbnormalTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((StatusControl)d).UpdateStatus();
        }
    }
}
