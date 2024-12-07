using System;
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
    public class QPopup : Popup
    {
        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);
            var d = e.OriginalSource as DependencyObject;
            if (CheckIsClose(d))
            {
                Task.Delay(50).ContinueWith((task) =>
                {
                    ClosePopup();
                });
            }
        }

        public static bool CheckIsClose(DependencyObject d)
        {
            DependencyObject control = d;
            while (control != null)
            {
                if (control is Popup)
                {
                    break;
                }
                else if (control is CheckBox || control is TextBox)
                {
                    return false;
                }

                control = control.GetParent();
            }
            return true;
        }

        private void ClosePopup()
        {
            if (CheckAccess())
            {
                if (IsOpen)
                {
                    SetCurrentValue(IsOpenProperty, false);
                }
            }
            else
            {
                Dispatcher.Invoke(ClosePopup);
            }
        }
    }
}
