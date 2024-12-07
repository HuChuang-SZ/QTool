using NPOI.HSSF.Record;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace QTool.Controls
{
    [TemplatePart(Name = PART_Up, Type = typeof(RepeatButton))]
    [TemplatePart(Name = PART_Down, Type = typeof(RepeatButton))]
    public class QNumber : Control
    {
        protected const string PART_Up = nameof(PART_Up);
        protected const string PART_Down = nameof(PART_Down);
        static QNumber()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(QNumber), new FrameworkPropertyMetadata(typeof(QNumber)));
        }
        private RepeatButton _up, _down;
        public override void OnApplyTemplate()
        {
            if (_up != null)
            {
                _up.Click -= Up_Click;
            }

            if (_down != null)
            {
                _down.Click -= Down_Click;
            }

            base.OnApplyTemplate();
            _up = GetTemplateChild(PART_Up) as RepeatButton;
            _down = GetTemplateChild(PART_Down) as RepeatButton;

            if (_up != null)
            {
                _up.Click += Up_Click;
            }
            if (_down != null)
            {
                _down.Click += Down_Click;
            }
        }

        private void Down_Click(object sender, RoutedEventArgs e)
        {
            Increment();
        }

        private void Up_Click(object sender, RoutedEventArgs e)
        {
            Decrement();
        }

        public IEnumerable ViewItems
        {
            get { return (IEnumerable)GetValue(ViewItemsProperty); }
        }

        // Using a DependencyProperty as the backing store for ViewItems.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewItemsProperty =
            DependencyProperty.Register("ViewItems", typeof(IEnumerable), typeof(QNumber), new PropertyMetadata(new int[] { -2, -1, 0, 1, 2 }));




        public int Value
        {
            get { return (int)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(int), typeof(QNumber), new PropertyMetadata(0, OnValueChanged));

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((QNumber)d).OnValueChanged((int)e.NewValue, (int)e.OldValue);
        }

        protected void OnValueChanged(int newValue, int oldValue)
        {
            RefreshViewItems();
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler ValueChanged;

        private void RefreshViewItems()
        {
            var viewItems = GetViewItems(Value).ToArray();
            SetCurrentValue(ViewItemsProperty, viewItems);
        }

        private IEnumerable<object> GetViewItems(int value)
        {
            int max = MaxValue, min = MinValue, viewCount = ViewCount;
            var startValue = value - (viewCount - 1) / 2;
            for (int i = 0; i < viewCount; i++)
            {
                var val = startValue + i;
                if (val >= min && val <= max)
                {
                    yield return val;
                }
                else
                {
                    yield return null;
                }
            }
        }

        public int MinValue
        {
            get { return (int)GetValue(MinValueProperty); }
            set { SetValue(MinValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Min.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinValueProperty =
            DependencyProperty.Register("MinValue", typeof(int), typeof(QNumber), new PropertyMetadata(int.MinValue, OnMinValueChanged));

        private static void OnMinValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((QNumber)d).RefreshViewItems();
        }

        public int MaxValue
        {
            get { return (int)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Max.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register("MaxValue", typeof(int), typeof(QNumber), new PropertyMetadata(int.MaxValue, OnMaxValueChanged));

        private static void OnMaxValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((QNumber)d).RefreshViewItems();
        }



        public int ViewCount
        {
            get { return (int)GetValue(ViewCountProperty); }
            set { SetValue(ViewCountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ViewCount.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewCountProperty =
            DependencyProperty.Register("ViewCount", typeof(int), typeof(QNumber), new PropertyMetadata(5, OnViewCountChanged));

        private static void OnViewCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((QNumber)d).RefreshViewItems();
        }



        public bool IsCircle
        {
            get { return (bool)GetValue(IsCircleProperty); }
            set { SetValue(IsCircleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsCircle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsCircleProperty =
            DependencyProperty.Register("IsCircle", typeof(bool), typeof(QNumber), new PropertyMetadata(false));

        protected override void OnPreviewMouseWheel(MouseWheelEventArgs e)
        {
            base.OnPreviewMouseWheel(e);
            if (e.Delta < 0)
            {
                Increment();
            }
            else if (e.Delta > 0)
            {
                Decrement();
            }
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
        }


        protected void Decrement()
        {
            var val = Value - 1;
            if (val < MinValue)
            {
                if (IsCircle)
                    val = MaxValue;
                else
                    val = MinValue;
            }

            SetCurrentValue(ValueProperty, val);
        }

        protected void Increment()
        {
            var val = Value + 1;
            if (val > MaxValue)
            {
                if (IsCircle)
                    val = MinValue;
                else
                    val = MaxValue;
            }
            SetCurrentValue(ValueProperty, val);
        }

    }
}
