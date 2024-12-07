using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace QTool.Controls
{
    [DefaultProperty(nameof(Content))]
    [ContentProperty(nameof(Content))]
    //[TemplatePart(Name = PART_Popup, Type = typeof(Popup))]
    public class PopupControl : HeaderedContentControl
    {
        //private const string PART_Popup = nameof(PART_Popup);
        static PopupControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PopupControl), new FrameworkPropertyMetadata(typeof(PopupButton)));
        }


        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsOpen.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(PopupControl), new PropertyMetadata(false));


        #region 弹出层开启/关闭逻辑
        public PopupMode PopupMode
        {
            get { return (PopupMode)GetValue(PopupModeProperty); }
            set { SetValue(PopupModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PopupMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PopupModeProperty =
            DependencyProperty.Register("PopupMode", typeof(PopupMode), typeof(PopupControl), new PropertyMetadata(PopupMode.Click));



        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            if (PopupMode == PopupMode.Click)
            {
                DelayExecute(() =>
                {
                    if (IsMouseOver)
                    {
                        SetCurrentValue(IsOpenProperty, !IsOpen);
                    }
                }, DelayMilliseconds);
            }
        }

        /// <summary>
        /// 延时开启弹出层
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            if (PopupMode == PopupMode.Move)
            {
                DelayExecute(() =>
                {
                    if (IsMouseOver)
                    {
                        SetCurrentValue(IsOpenProperty, true);
                    }
                }, DelayMilliseconds);
            }
        }

        /// <summary>
        /// 延时关闭弹出层
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            DelayExecute(() =>
            {
                if (!IsMouseOver)
                    SetCurrentValue(IsOpenProperty, false);

            }, DelayMilliseconds * 2);
        }


        /// <summary>
        /// 延迟执行代码
        /// </summary>
        /// <param name="method">执行方法</param>
        /// <param name="delayMilliseconds">延时毫秒</param>
        private void DelayExecute(Action method, int delayMilliseconds)
        {
            Task.Factory.StartNew(() =>
            {
                System.Threading.Thread.Sleep(delayMilliseconds);
                Dispatcher.BeginInvoke(method);
            });
        }

        /// <summary>
        /// 延时毫秒（默认100毫秒）
        /// </summary>
        public int DelayMilliseconds
        {
            get { return (int)GetValue(DelayMillisecondsProperty); }
            set { SetValue(DelayMillisecondsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DelayMilliseconds.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DelayMillisecondsProperty =
            DependencyProperty.Register("DelayMilliseconds", typeof(int), typeof(PopupControl), new PropertyMetadata(100));
        #endregion 弹出层开启/关闭逻辑


        public PlacementMode PopupPlacement
        {
            get { return (PlacementMode)GetValue(PopupPlacementProperty); }
            set { SetValue(PopupPlacementProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PopupPlacement.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PopupPlacementProperty =
            DependencyProperty.Register("PopupPlacement", typeof(PlacementMode), typeof(PopupControl), new PropertyMetadata(PlacementMode.Bottom));



        public double PopupVerticalOffset
        {
            get { return (double)GetValue(PopupVerticalOffsetProperty); }
            set { SetValue(PopupVerticalOffsetProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PopupVerticalOffset.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PopupVerticalOffsetProperty =
            DependencyProperty.Register("PopupVerticalOffset", typeof(double), typeof(PopupControl), new PropertyMetadata(1d));
    }
}

