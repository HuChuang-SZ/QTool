using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace QTool.Controls
{
    public class QItemsControl : HeaderedItemsControl
    {
        static QItemsControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(QItemsControl), new FrameworkPropertyMetadata(typeof(QItemsControl)));
        }


        public object Footer
        {
            get { return (object)GetValue(FooterProperty); }
            set { SetValue(FooterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Footer.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FooterProperty =
            DependencyProperty.Register("Footer", typeof(object), typeof(QItemsControl), new PropertyMetadata(null, OnFooterChanged));

        private static void OnFooterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((QItemsControl)d).OnFooterChanged(e.NewValue, e.OldValue);
        }

        private void OnFooterChanged(object newValue, object oldValue)
        {
            SetCurrentValue(HasFooterProperty, newValue != null);
        }

        public bool HasFooter
        {
            get { return (bool)GetValue(HasFooterProperty); }
        }

        // Using a DependencyProperty as the backing store for HasFooter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HasFooterProperty =
            DependencyProperty.Register("HasFooter", typeof(bool), typeof(QItemsControl), new PropertyMetadata(false));


    }
}
