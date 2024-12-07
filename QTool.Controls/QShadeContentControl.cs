using iTextSharp.text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace QTool.Controls
{
    [TemplatePart(Name = PART_ShadeBd, Type = typeof(Border))]
    [TemplatePart(Name = PART_ContentBd, Type = typeof(Border))]
    [TemplatePart(Name = PART_CloseBtn, Type = typeof(Button))]
    public class QShadeContentControl : ContentControl
    {
        private const string PART_ShadeBd = nameof(PART_ShadeBd);
        private const string PART_ContentBd = nameof(PART_ContentBd);
        private const string PART_CloseBtn = nameof(PART_CloseBtn);
        static QShadeContentControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(QShadeContentControl), new FrameworkPropertyMetadata(typeof(QShadeContentControl)));
        }

        public QShadeContentControl()
        {
            
        }

        private Border _shadeBd;
        private Border _contentBd;
        private Button _closeBtn;
        public override void OnApplyTemplate()
        {
            if (_shadeBd != null)
            {
                _shadeBd.MouseLeftButtonDown -= ShadeBd_MouseLeftButtonDown;
            }

            if (_closeBtn != null)
            {
                _closeBtn.Click -= CloseBtn_Click;
            }
            base.OnApplyTemplate();
            _shadeBd = GetTemplateChild(PART_ShadeBd) as Border;
            _contentBd = GetTemplateChild(PART_ContentBd) as Border;
            _closeBtn = GetTemplateChild(PART_CloseBtn) as Button;
            if (_shadeBd != null)
            {
                _shadeBd.MouseLeftButtonDown += ShadeBd_MouseLeftButtonDown;
            }

            if (_closeBtn != null)
            {
                _closeBtn.Click += CloseBtn_Click;
            }

            ResizeContentBd();
        }

        private void ResizeContentBd()
        {
            if (_contentBd != null)
            {
                _contentBd.MaxWidth = ActualWidth * 0.9;
                _contentBd.MaxHeight = ActualHeight * 0.9;
            }
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            ResizeContentBd();
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CloseMode.HasFlag(ShadeCloseMode.OnlyCloseBtn))
            {
                Close();
            }
        }

        private void ShadeBd_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (CloseMode.HasFlag(ShadeCloseMode.OnlyBlankClick))
            {
                Close();
            }
        }


        //protected override void OnPreviewKeyDown(KeyEventArgs e)
        //{
        //    base.OnPreviewKeyDown(e);
        //    if (e.Key == Key.Escape && CloseMode.HasFlag(ShadeCloseMode.OnlyEsc))
        //    {
        //        Close();
        //    }
        //}

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(QShadeContentControl), new PropertyMetadata(string.Empty));


        public ShadeCloseMode CloseMode
        {
            get { return (ShadeCloseMode)GetValue(CloseModeProperty); }
            set { SetValue(CloseModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CloseMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CloseModeProperty =
            DependencyProperty.Register("CloseMode", typeof(ShadeCloseMode), typeof(QShadeContentControl), new PropertyMetadata(ShadeCloseMode.Both, OnCloseModeChanged));

        private static void OnCloseModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((QShadeContentControl)d).OnCloseModeChanged();
        }

        protected void OnCloseModeChanged()
        {
            SetCurrentValue(CanCloseBtnProperty, CloseMode.HasFlag(ShadeCloseMode.OnlyCloseBtn));
        }



        public bool CanCloseBtn
        {
            get { return (bool)GetValue(CanCloseBtnProperty); }
        }

        // Using a DependencyProperty as the backing store for CanCloseBtn.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CanCloseBtnProperty =
            DependencyProperty.Register("CanCloseBtn", typeof(bool), typeof(QShadeContentControl), new PropertyMetadata(true));

        #region Closed Event
        private EventHandler _closed;
        public event EventHandler Closed
        {
            add
            {
                _closed += value;
            }
            remove
            {
                _closed -= value;
            }
        }



        private void Close()
        {
            SetCurrentValue(ContentProperty, null);
            _closed?.Invoke(this, EventArgs.Empty);
        }
        #endregion Closed Event
    }
}
