using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace QTool.Controls
{
    [TemplatePart(Name = PART_MinimizeBtn, Type = typeof(Button))]
    [TemplatePart(Name = PART_MaximizeBtn, Type = typeof(Button))]
    [TemplatePart(Name = PART_CloseBtn, Type = typeof(Button))]
    public class QWindow : Window
    {
        private const string PART_MinimizeBtn = nameof(PART_MinimizeBtn);
        private const string PART_MaximizeBtn = nameof(PART_MaximizeBtn);
        private const string PART_CloseBtn = nameof(PART_CloseBtn);

        static QWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(QWindow), new FrameworkPropertyMetadata(typeof(QWindow)));
        }

        public QWindow()
        {
            SetCurrentValue(MaxHeightProperty, SystemParameters.MaximizedPrimaryScreenHeight);
        }

        public DateTime CreateAt { get; } = DateTime.Now;

        public virtual bool IsMain { get { return false; } }

        private Button _minimizeBtn, _maximizeBtn, _closeBtn;
        public override void OnApplyTemplate()
        {
            if (_minimizeBtn != null)
            {
                _minimizeBtn.Click -= _minimizeBtn_Click;
            }

            if (_maximizeBtn != null)
            {
                _maximizeBtn.Click -= _maximizeBtn_Click;
            }

            if (_closeBtn != null)
            {
                _closeBtn.Click -= _closeBtn_Click;
            }

            base.OnApplyTemplate();

            _minimizeBtn = GetTemplateChild(PART_MinimizeBtn) as Button;
            _maximizeBtn = GetTemplateChild(PART_MaximizeBtn) as Button;
            _closeBtn = GetTemplateChild(PART_CloseBtn) as Button;

            if (_minimizeBtn != null)
            {
                _minimizeBtn.Click += _minimizeBtn_Click;
            }

            if (_maximizeBtn != null)
            {
                _maximizeBtn.Click += _maximizeBtn_Click;
            }

            if (_closeBtn != null)
            {
                _closeBtn.Click += _closeBtn_Click;
            }
        }

        private void _closeBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void _maximizeBtn_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
                SetCurrentValue(WindowStateProperty, WindowState.Maximized);
            else if (WindowState == WindowState.Maximized)
                SetCurrentValue(WindowStateProperty, WindowState.Normal);
        }

        private void _minimizeBtn_Click(object sender, RoutedEventArgs e)
        {
            SetCurrentValue(WindowStateProperty, WindowState.Minimized);
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            //监控分辨率改变时 触发
            if (WindowState == WindowState.Maximized)
            {
                Task.Factory.StartNew(() =>
                {
                    Task.Delay(100).Wait();
                    ResetMaxHeight();
                });
            }
        }

        /// <summary>
        /// 分辨率改变时 更新 MaxHeight
        /// </summary>
        private void ResetMaxHeight()
        {
            if (CheckAccess())
            {
                if (WindowState == WindowState.Maximized && MaxHeight != SystemParameters.MaximizedPrimaryScreenHeight)
                {
                    WindowState = WindowState.Normal;
                    SetCurrentValue(MaxHeightProperty, SystemParameters.MaximizedPrimaryScreenHeight);
                    WindowState = WindowState.Maximized;
                }
            }
            else
            {
                Dispatcher.Invoke(new Action(ResetMaxHeight));
            }
        }



        public bool IsWaiting
        {
            get { return (bool)GetValue(IsWaitingProperty); }
            set { SetValue(IsWaitingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsWaiting.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsWaitingProperty =
            DependencyProperty.Register("IsWaiting", typeof(bool), typeof(QWindow), new PropertyMetadata(false));



        public string WaitMsg
        {
            get { return (string)GetValue(WaitMsgProperty); }
            set { SetValue(WaitMsgProperty, value); }
        }

        // Using a DependencyProperty as the backing store for WaitMsg.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WaitMsgProperty =
            DependencyProperty.Register("WaitMsg", typeof(string), typeof(QWindow), new PropertyMetadata(null));



        [Bindable(true)]
        [Category("Content")]
        [Localizability(LocalizationCategory.Label)]
        public object Header
        {
            get { return (object)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(object), typeof(QWindow), new PropertyMetadata(null, OnHeaderChanged));




        public bool HasHeader
        {
            get { return (bool)GetValue(HasHeaderProperty); }
            private set { SetValue(HasHeaderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HasHeader.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HasHeaderProperty =
            DependencyProperty.Register("HasHeader", typeof(bool), typeof(QWindow), new PropertyMetadata(false));


        private static void OnHeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            QWindow window = (QWindow)d;
            window.SetValue(HasHeaderProperty, (e.NewValue != null));
            window.OnHeaderChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnHeaderChanged(object oldValue, object newValue)
        {
            RemoveLogicalChild(oldValue);
            AddLogicalChild(newValue);
        }



        [Bindable(true)]
        [Category("Content")]
        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeaderTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(QWindow), new PropertyMetadata(null, OnHeaderTemplateChanged));

        private static void OnHeaderTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            QWindow window = (QWindow)d;
            window.OnHeaderTemplateChanged((DataTemplate)e.OldValue, (DataTemplate)e.NewValue);
        }

        protected virtual void OnHeaderTemplateChanged(DataTemplate oldHeaderTemplate, DataTemplate newHeaderTemplate)
        {

        }


        [Bindable(true)]
        [Category("Content")]
        public DataTemplateSelector HeaderTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(HeaderTemplateSelectorProperty); }
            set { SetValue(HeaderTemplateSelectorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeaderTemplateSelector.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderTemplateSelectorProperty =
            DependencyProperty.Register("HeaderTemplateSelector", typeof(DataTemplateSelector), typeof(QWindow), new PropertyMetadata(null, OnHeaderTemplateSelectorChanged));


        private static void OnHeaderTemplateSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            QWindow window = (QWindow)d;
            window.OnHeaderTemplateSelectorChanged((DataTemplateSelector)e.OldValue, (DataTemplateSelector)e.NewValue);
        }

        protected virtual void OnHeaderTemplateSelectorChanged(DataTemplateSelector oldHeaderTemplateSelector, DataTemplateSelector newHeaderTemplateSelector)
        {
        }



        [Bindable(true)]
        [Category("Content")]
        public string HeaderStringFormat
        {
            get { return (string)GetValue(HeaderStringFormatProperty); }
            set { SetValue(HeaderStringFormatProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeaderStringFormat.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderStringFormatProperty =
            DependencyProperty.Register("HeaderStringFormat", typeof(string), typeof(QWindow), new PropertyMetadata(null, OnHeaderStringFormatChanged));


        private static void OnHeaderStringFormatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            QWindow window = (QWindow)d;
            window.OnHeaderStringFormatChanged((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual void OnHeaderStringFormatChanged(string oldHeaderStringFormat, string newHeaderStringFormat)
        {
        }
    }
}
