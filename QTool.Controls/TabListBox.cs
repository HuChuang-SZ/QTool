using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace QTool.Controls
{
    public class TabListBox : ListBox
    {

        static TabListBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TabListBox), new FrameworkPropertyMetadata(typeof(TabListBox)));
        }

        public double ItemWidth
        {
            get { return (double)GetValue(ItemWidthProperty); }
        }

        // Using a DependencyProperty as the backing store for ItemWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemWidthProperty =
            DependencyProperty.Register("ItemWidth", typeof(double), typeof(TabListBox), new PropertyMetadata(180d));


        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            CalcuateItemWidth();
        }

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);

            CalcuateItemWidth();
        }

        private void CalcuateItemWidth()
        {

            if (IsInitialized)
            {
                var itemWidth = ActualWidth / Items.Count;
                if (itemWidth > 180)
                {
                    itemWidth = 180;
                }

                if (itemWidth < 30)
                {
                    itemWidth = (ActualWidth - 60) / (Items.Count - 2);
                }

                SetCurrentValue(ItemWidthProperty, itemWidth);
            }
        }
    }
}
