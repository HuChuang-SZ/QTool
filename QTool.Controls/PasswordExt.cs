using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace QTool.Controls
{
    public class PasswordExt
    {

        #region 密码框支持绑定
        public static bool GetIsBind(PasswordBox obj)
        {
            return (bool)obj.GetValue(IsBindProperty);
        }

        public static void SetIsBind(PasswordBox obj, bool value)
        {
            obj.SetValue(IsBindProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsBind.  This enables animation, styling, binding, etc...
        private static readonly DependencyProperty IsBindProperty =
            DependencyProperty.RegisterAttached("IsBind", typeof(bool), typeof(PasswordExt), new UIPropertyMetadata(false, OnIsBindChanged));

        private static void OnIsBindChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PasswordBox password = d as PasswordBox;
            if (password != null)
            {
                if ((bool)e.NewValue)
                    password.PasswordChanged += new RoutedEventHandler(password_PasswordChanged);
                else
                    password.PasswordChanged -= new RoutedEventHandler(password_PasswordChanged);
            }
        }

        public static string GetText(PasswordBox obj)
        {
            return (string)obj.GetValue(TextProperty);
        }

        public static void SetText(PasswordBox obj, string value)
        {
            obj.SetValue(TextProperty, value);
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.RegisterAttached("Text", typeof(string), typeof(PasswordExt), new UIPropertyMetadata("", OnTextChanged));



        private static void OnTextChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is PasswordBox)
            {
                PasswordBox password = sender as PasswordBox;
                string strPassword = e.NewValue as string;
                if (password.Password != strPassword)
                {
                    password.Password = strPassword;
                }
            }
        }

        static void password_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox)
            {
                PasswordBox password = sender as PasswordBox;
                if (GetText(password) != password.Password)
                {
                    SetText(password, password.Password);
                }
            }
        }
        #endregion 密码框支持绑定
    }
}
