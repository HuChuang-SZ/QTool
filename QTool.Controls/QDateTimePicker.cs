using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace QTool.Controls
{

    [TemplatePart(Name = PART_TextBox, Type = typeof(TextBox))]
    [TemplatePart(Name = PART_Calendar, Type = typeof(Calendar))]
    [TemplatePart(Name = PART_Hour, Type = typeof(QNumber))]
    [TemplatePart(Name = PART_Minute, Type = typeof(QNumber))]
    [TemplatePart(Name = PART_Second, Type = typeof(QNumber))]
    public class QDateTimePicker : Control
    {
        private const string PART_TextBox = nameof(PART_TextBox);
        private const string PART_Calendar = nameof(PART_Calendar);
        private const string PART_Hour = nameof(PART_Hour);
        private const string PART_Minute = nameof(PART_Minute);
        private const string PART_Second = nameof(PART_Second);

        static QDateTimePicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(QDateTimePicker), new FrameworkPropertyMetadata(typeof(QDateTimePicker)));
        }

        public QDateTimePicker()
        {
            CommandBindings.Add(new CommandBinding(QCommands.Select, Select_Executed));
            CommandBindings.Add(new CommandBinding(QCommands.OK, OK_Executed));
        }

        private void OK_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SetCurrentValue(IsOpenProperty, false);
        }

        private void Select_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter is string type)
            {
                var nowTime = DateTime.Now;
                switch (type)
                {
                    case "Now":
                        SetDateTime(nowTime, true);
                        break;
                    case "Today":
                        SetDateTime(nowTime.Date, true);
                        break;
                    case "LastDay":
                        SetDateTime(nowTime.Date.AddDays(-1), true);
                        break;
                    case "Last3Days":
                        SetDateTime(nowTime.Date.AddDays(-3), true);
                        break;
                    case "Last7Days":
                        SetDateTime(nowTime.Date.AddDays(-7), true);
                        break;
                    case "Last15Days":
                        SetDateTime(nowTime.Date.AddDays(-15), true);
                        break;
                    case "Last24Hours":
                        SetDateTime(nowTime.AddHours(-24), true);
                        break;
                    case "Last48Hours":
                        SetDateTime(nowTime.AddHours(-48), true);
                        break;
                    case "Last72Hours":
                        SetDateTime(nowTime.AddHours(-72), true);
                        break;
                    default:
                        return;
                }
                SetCurrentValue(IsOpenProperty, false);
            }
        }

        #region 弹出层开启/关闭逻辑
        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsOpen.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(QDateTimePicker), new PropertyMetadata(false));


        //protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        //{
        //    base.OnMouseLeftButtonDown(e);
        //    if (e.OriginalSource is DependencyObject d && !d.TryFindParent(out Popup popup))
        //    {
        //        DelayExecute(() =>
        //        {
        //            if (IsMouseOver)
        //                SetCurrentValue(IsOpenProperty, !IsOpen);
        //        }, DelayMilliseconds);
        //    }
        //}

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);

            DelayExecute(() =>
            {
                if (IsMouseOver && (_textBox != null && _textBox.IsFocused))
                    SetCurrentValue(IsOpenProperty, true);

            }, DelayMilliseconds * 1);
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
                TryClosePopup();

            }, DelayMilliseconds * 5);
        }

        private void TryClosePopup()
        {
            if (!IsMouseOver)
                SetCurrentValue(IsOpenProperty, false);
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
            DependencyProperty.Register("DelayMilliseconds", typeof(int), typeof(QDateTimePicker), new PropertyMetadata(100));



        public PlacementMode PopupPlacement
        {
            get { return (PlacementMode)GetValue(PopupPlacementProperty); }
            set { SetValue(PopupPlacementProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PopupPlacement.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PopupPlacementProperty =
            DependencyProperty.Register("PopupPlacement", typeof(PlacementMode), typeof(QDateTimePicker), new PropertyMetadata(PlacementMode.Bottom));



        public double PopupVerticalOffset
        {
            get { return (double)GetValue(PopupVerticalOffsetProperty); }
            set { SetValue(PopupVerticalOffsetProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PopupVerticalOffset.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PopupVerticalOffsetProperty =
            DependencyProperty.Register("PopupVerticalOffset", typeof(double), typeof(QDateTimePicker), new PropertyMetadata(1d));
        #endregion 弹出层开启/关闭逻辑



        public string Placeholder
        {
            get { return (string)GetValue(PlaceholderProperty); }
            set { SetValue(PlaceholderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Placeholder.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PlaceholderProperty =
            DependencyProperty.Register("Placeholder", typeof(string), typeof(QDateTimePicker), new PropertyMetadata("选择时间"));




        public DateTime? SelectedDateTime
        {
            get { return (DateTime?)GetValue(SelectedDateTimeProperty); }
            set { SetValue(SelectedDateTimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedDateTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedDateTimeProperty =
            DependencyProperty.Register("SelectedDateTime", typeof(DateTime?), typeof(QDateTimePicker), new PropertyMetadata(null, OnSelectedDateTimeChanged));

        private static void OnSelectedDateTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((QDateTimePicker)d).BindDateTime();
        }

        private Calendar _calendar;
        private QNumber _hour, _minute, _second;
        private TextBox _textBox;
        public override void OnApplyTemplate()
        {
            if (_textBox != null)
            {
                _textBox.GotFocus -= TextBox_GotFocus; ;
                _textBox.LostFocus -= TextBox_LostFocus;
                _textBox.TextChanged -= TextBox_TextChanged;
            }

            if (_calendar != null)
            {
                _calendar.PreviewMouseUp -= Calendar_PreviewMouseUp;
                _calendar.SelectedDatesChanged -= Calendar_SelectedDatesChanged;
            }

            if (_hour != null)
            {
                _hour.ValueChanged -= QNumber_ValueChanged;
            }

            if (_minute != null)
            {
                _minute.ValueChanged -= QNumber_ValueChanged;
            }

            if (_second != null)
            {
                _second.ValueChanged -= QNumber_ValueChanged;
            }

            base.OnApplyTemplate();
            _textBox = GetTemplateChild(PART_TextBox) as TextBox;
            _calendar = GetTemplateChild(PART_Calendar) as Calendar;
            _hour = GetTemplateChild(PART_Hour) as QNumber;
            _minute = GetTemplateChild(PART_Minute) as QNumber;
            _second = GetTemplateChild(PART_Second) as QNumber;

            if (_textBox != null)
            {
                _textBox.GotFocus += TextBox_GotFocus;
                _textBox.LostFocus += TextBox_LostFocus;
                _textBox.TextChanged += TextBox_TextChanged;
            }

            if (_calendar != null)
            {
                _calendar.PreviewMouseUp += Calendar_PreviewMouseUp;
                _calendar.SelectedDatesChanged += Calendar_SelectedDatesChanged;
            }

            if (_hour != null)
            {
                _hour.ValueChanged += QNumber_ValueChanged;
            }

            if (_minute != null)
            {
                _minute.ValueChanged += QNumber_ValueChanged;
            }

            if (_second != null)
            {
                _second.ValueChanged += QNumber_ValueChanged;
            }

            BindDateTime();
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            if (!e.Handled && _textBox != null)
            {
                _textBox.SelectAll();
                _textBox.Focus();
                e.Handled = true;
            }
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            Focus();
        }

        private void QNumber_ValueChanged(object sender, EventArgs e)
        {
            SetDateTime(GetDateTimeByPopup(), true);
        }

        private void Calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            SetDateTime(GetDateTimeByPopup(), true);
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (DateTime.TryParse(_textBox.Text, out DateTime dateTime))
            {
                SetDateTime(dateTime, false);
            }
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            SetCurrentValue(IsOpenProperty, true);
            _textBox.SelectAll();
            e.Handled = true;
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (DateTime.TryParse(_textBox.Text, out DateTime dateTime))
            {
                SetDateTime(dateTime, true);
            }
            else
            {
                SetDateTime(null, true);
            }
            TryClosePopup();
        }

        private void _btnNow_Click(object sender, RoutedEventArgs e)
        {
            SetCurrentValue(SelectedDateTimeProperty, DateTime.Now);
            SetCurrentValue(IsOpenProperty, false);
        }

        private void _btnOk_Click(object sender, RoutedEventArgs e)
        {
            SetCurrentValue(IsOpenProperty, false);
        }

        private void Calendar_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (sender != null && !(e.OriginalSource is Button))
            {
                UIElement el = sender as UIElement;
                el.ReleaseStylusCapture();
            }
        }

        private Dictionary<DependencyProperty, bool> _isHandlerSuspended = new Dictionary<DependencyProperty, bool>();
        private bool IsHandlerSuspended(DependencyProperty property)
        {
            if (_isHandlerSuspended != null)
            {
                return _isHandlerSuspended.ContainsKey(property);
            }

            return false;
        }

        private void SetIsHandlerSuspended(DependencyProperty property, bool value)
        {
            if (value)
            {
                if (_isHandlerSuspended == null)
                {
                    _isHandlerSuspended = new Dictionary<DependencyProperty, bool>(2);
                }

                _isHandlerSuspended[property] = true;
            }
            else if (_isHandlerSuspended != null)
            {
                _isHandlerSuspended.Remove(property);
            }
        }

        private void SetDateTime(DateTime? dateTime, bool bindTextBox)
        {
            if (!IsHandlerSuspended(SelectedDateTimeProperty))
            {
                SetIsHandlerSuspended(SelectedDateTimeProperty, true);

                SetCurrentValue(SelectedDateTimeProperty, dateTime);

                if (bindTextBox)
                    BindTextBox(dateTime);

                SetIsHandlerSuspended(SelectedDateTimeProperty, false);
            }
        }

        private DateTime? GetDateTimeByPopup()
        {
            var date = _calendar?.SelectedDate;
            var hour = _hour?.Value;
            var minute = _minute?.Value;
            var second = _second?.Value;
            if (date.HasValue)
            {
                return new DateTime(date.Value.Year, date.Value.Month, date.Value.Day, hour ?? 0, minute ?? 0, second ?? 0);
            }
            else
            {
                return null;
            }
        }

        private void BindDateTime()
        {
            if (CheckAccess())
            {
                if (!IsHandlerSuspended(SelectedDateTimeProperty))
                {
                    SetIsHandlerSuspended(SelectedDateTimeProperty, true);

                    var selectedDateTime = SelectedDateTime;
                    BindPopup(selectedDateTime ?? DateTime.Now.Date);
                    BindTextBox(selectedDateTime);

                    SetIsHandlerSuspended(SelectedDateTimeProperty, false);
                }
            }
            else
            {
                Dispatcher.Invoke(BindDateTime);
            }
        }

        private void BindPopup(DateTime dateTime)
        {
            if (_calendar != null)
                _calendar.SelectedDate = _calendar.DisplayDate = dateTime.Date;

            if (_hour != null)
                _hour.Value = dateTime.Hour;

            if (_minute != null)
                _minute.Value = dateTime.Minute;

            if (_second != null)
                _second.Value = dateTime.Second;
        }

        private void BindTextBox(DateTime? selectedDateTime)
        {
            if (_textBox != null)
            {
                if (selectedDateTime.HasValue)
                {
                    _textBox.Text = DateTimeToString(selectedDateTime.Value);
                }
                else
                {
                    _textBox.Text = "";
                }
            }
        }

        private string DateTimeToString(DateTime value)
        {
            return value.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}
