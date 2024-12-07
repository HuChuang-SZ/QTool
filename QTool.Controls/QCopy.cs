using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Documents;

namespace QTool.Controls
{
    public class QCopy
    {
        public static bool GetIsEnabled(TextBlock obj)
        {
            return (bool)obj.GetValue(IsEnabledProperty);
        }

        public static void SetIsEnabled(TextBlock obj, bool value)
        {
            obj.SetValue(IsEnabledProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(QCopy), new PropertyMetadata(false, OnIsEnabledChanged));

        private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)

        {
            if ((bool)e.NewValue)
            {
                var elem = d as UIElement;
                if (elem != null)
                {
                    elem.MouseEnter += CopyElement_MouseEnter;
                    elem.MouseLeave += CopyElement_MouseLeave;
                    elem.MouseLeftButtonUp += CopyElement_MouseLeftButtonUp;
                }
            }
            else
            {
                var elem = d as UIElement;
                if (elem != null)
                {
                    elem.MouseEnter -= CopyElement_MouseEnter;
                    elem.MouseLeave -= CopyElement_MouseLeave;
                    elem.MouseLeftButtonUp -= CopyElement_MouseLeftButtonUp;
                }
            }
        }

        private static void CopyElement_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var elem = sender as TextBlock;
            if (elem != null)
            {
                OpenCopyPopup(elem);

                try
                {
                    Clipboard.SetText(elem.Text);
                    ((ContentControl)_copyPopup.Child).Content = "复制成功";
                }
                catch
                {
                    ((ContentControl)_copyPopup.Child).Content = "复制失败";
                }
            }
        }

        private static void CopyElement_MouseEnter(object sender, MouseEventArgs e)
        {
            var elem = sender as TextBlock;
            if (elem != null)
            {
                OpenCopyPopup(elem);
            }
        }

        private static void CopyElement_MouseLeave(object sender, MouseEventArgs e)
        {
            var elem = sender as TextBlock;
            if (elem != null)
            {
                if (_copyPopup.PlacementTarget == elem)
                    _copyPopup.IsOpen = false;
            }
        }

        private static Popup _copyPopup;
        private static void OpenCopyPopup(TextBlock elem)
        {
            if (_copyPopup == null)
            {
                _copyPopup = new Popup();
                _copyPopup.AllowsTransparency = true;
                _copyPopup.Child = new ContentControl() { Content = "点击复制", Style = _copyPopup.FindResource("CopyPopupContent") as Style };
                _copyPopup.Placement = PlacementMode.Top;
                _copyPopup.PopupAnimation = PopupAnimation.Slide;
            }
            else
            {
                ((ContentControl)_copyPopup.Child).Content = "点击复制";
            }

            if (_copyPopup.PlacementTarget != elem)
            {
                _copyPopup.PlacementTarget = elem;
                _copyPopup.HorizontalOffset = (elem.ActualWidth - 62) / 2;
            }
            if (!_copyPopup.IsOpen)
                _copyPopup.IsOpen = true;
        }
    }
}
