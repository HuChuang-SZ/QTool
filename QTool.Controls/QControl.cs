using QTool.Controls.Models;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace QTool.Controls
{
    public static class QControl
    {
        public static string GetPlaceholder(DependencyObject obj)
        {
            return (string)obj.GetValue(PlaceholderProperty);
        }

        public static void SetPlaceholder(DependencyObject obj, string value)
        {
            obj.SetValue(PlaceholderProperty, value);
        }

        // Using a DependencyProperty as the backing store for Placeholder.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PlaceholderProperty =
            DependencyProperty.RegisterAttached("Placeholder", typeof(string), typeof(QControl), new PropertyMetadata(null));







        public static string GetUnit(TextBox obj)
        {
            return (string)obj.GetValue(UnitProperty);
        }

        public static void SetUnit(TextBox obj, string value)
        {
            obj.SetValue(UnitProperty, value);
        }

        // Using a DependencyProperty as the backing store for Unit.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UnitProperty =
            DependencyProperty.RegisterAttached("Unit", typeof(string), typeof(QControl), new PropertyMetadata(null));




        public static bool GetIsCustiomChecked(CheckBox obj)
        {
            return (bool)obj.GetValue(IsCustiomCheckedProperty);
        }

        public static void SetIsCustiomChecked(CheckBox obj, bool value)
        {
            obj.SetValue(IsCustiomCheckedProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsCustiomChecked.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsCustiomCheckedProperty =
            DependencyProperty.RegisterAttached("IsCustiomChecked", typeof(bool), typeof(QControl), new PropertyMetadata(false, OnIsCustiomCheckedChanged));

        private static void OnIsCustiomCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is bool val && d is CheckBox checkBox)
            {
                if (val)
                {
                    checkBox.PreviewMouseLeftButtonDown += CheckBox_PreviewMouseLeftButtonDown;
                }
                else
                {
                    checkBox.PreviewMouseLeftButtonDown -= CheckBox_PreviewMouseLeftButtonDown;
                }
            }
        }

        private static void CheckBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                switch (checkBox.IsChecked)
                {
                    case true:
                        checkBox.IsChecked = false;
                        break;
                    case false:
                        if (checkBox.IsThreeState)
                            checkBox.IsChecked = null;
                        else
                            checkBox.IsChecked = true;
                        break;
                    default:
                        checkBox.IsChecked = true;
                        break;
                }
                e.Handled = true;
            }
        }
    }
}
