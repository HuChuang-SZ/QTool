using NPOI.SS.Formula.Functions;
using Org.BouncyCastle.Utilities;
using QTool.Controls.Models;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace QTool.Controls
{
    public class QGridViewColumn : GridViewColumn
    {
        //public Guid Identity
        //{
        //    get { return (Guid)GetValue(IdentityProperty); }
        //    set { SetValue(IdentityProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for Identity.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty IdentityProperty =
        //    DependencyProperty.Register("Identity", typeof(Guid), typeof(QGridViewColumn), new PropertyMetadata(null));



        public string GroupName
        {
            get { return (string)GetValue(GroupNameProperty); }
            set { SetValue(GroupNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for GroupName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GroupNameProperty =
            DependencyProperty.Register("GroupName", typeof(string), typeof(QGridViewColumn), new PropertyMetadata(null));



        #region 适配宽度
        public double FitWidth
        {
            get { return (double)GetValue(FitWidthProperty); }
            set { SetValue(FitWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Width.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FitWidthProperty =
            DependencyProperty.RegisterAttached("FitWidth", typeof(double), typeof(QGridViewColumn), new PropertyMetadata(double.NaN));




        public double FitMinWidth
        {
            get { return (double)GetValue(FitMinWidthProperty); }
            set { SetValue(FitMinWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FitMinWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FitMinWidthProperty =
            DependencyProperty.Register("FitMinWidth", typeof(double), typeof(QGridViewColumn), new PropertyMetadata(double.NaN));


        public double FitMaxWidth
        {
            get { return (double)GetValue(FitMaxWidthProperty); }
            set { SetValue(FitMaxWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FitMaxWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FitMaxWidthProperty =
            DependencyProperty.Register("FitMaxWidth", typeof(double), typeof(QGridViewColumn), new PropertyMetadata(double.NaN));
        #endregion 适配宽度

        #region 排序
        public ListSortDirection? Direction
        {
            get
            {
                return (ListSortDirection?)GetValue(DirectionProperty);
            }
            set
            {
                SetValue(DirectionProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for Direction.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DirectionProperty =
            DependencyProperty.RegisterAttached("Direction", typeof(ListSortDirection?), typeof(QGridViewColumn), new PropertyMetadata(null));



        public string SortFields
        {
            get
            {
                return (string)GetValue(SortFieldsProperty);
            }
            set
            {
                SetValue(SortFieldsProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for SortFields.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SortFieldsProperty =
            DependencyProperty.RegisterAttached("SortFields", typeof(string), typeof(QGridViewColumn), new PropertyMetadata(null));
        #endregion 排序

        #region 筛选
        public DataTemplate FilterTemplate
        {
            get
            {
                return (DataTemplate)GetValue(FilterTemplateProperty);
            }
            set
            {
                SetValue(FilterTemplateProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for FilterTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FilterTemplateProperty =
            DependencyProperty.RegisterAttached("FilterTemplate", typeof(DataTemplate), typeof(QGridViewColumn), new PropertyMetadata(null));


        public IFilterCondition Filter
        {
            get
            {
                return (IFilterCondition)GetValue(FilterProperty);
            }
            set
            {
                SetValue(FilterProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for Filter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FilterProperty =
            DependencyProperty.RegisterAttached("Filter", typeof(IFilterCondition), typeof(QGridViewColumn), new PropertyMetadata(null));

        public bool IsShowFilter
        {
            get
            {
                return (bool)GetValue(IsShowFilterProperty);
            }
            set
            {
                SetValue(IsShowFilterProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for IsShowFilter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsShowFilterProperty =
            DependencyProperty.RegisterAttached("IsShowFilter", typeof(bool), typeof(QGridViewColumn), new PropertyMetadata(false, OnIsShowFilterChanged));

        private static void OnIsShowFilterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var column = d as QGridViewColumn;
            if (column != null)
            {
                if ((bool)e.NewValue)
                {
                    var filter = column.Filter;
                    if (filter != null)
                    {
                        filter.CreateOptions(column);
                    }
                }
            }
        }
        #endregion 筛选
    }
}
