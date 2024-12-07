using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Threading;
using System.ComponentModel;
using System.Windows.Data;
using System.Collections.Specialized;

using System.Collections;
using QTool.Controls.Models;
using System.Diagnostics;
using System.Windows.Input;
using Org.BouncyCastle.Utilities;
using NPOI.SS.Formula.Functions;

namespace QTool.Controls
{
    public class QListView : ListView
    {
        static QListView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(QListView), new FrameworkPropertyMetadata(typeof(QListView)));
        }

        public QListView()
        {
            CommandBindings.Add(new CommandBinding(QCommands.SelectAll, SelectAll_Executed));
            DependencyPropertyDescriptor.FromProperty(ViewProperty, typeof(QListView)).AddValueChanged(this, OnViewChanged);
        }

        protected virtual void OnViewChanged(object sender, EventArgs e)
        {
            Task.Factory.StartNew(UpdateFitWidth);
        }

        private void SelectAll_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var isChecked = e.Parameter as bool?;
            if (isChecked.HasValue)
            {
                if (isChecked.Value)
                {
                    SelectAll();
                }
                else
                {
                    UnselectAll();
                }
            }
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            Task.Factory.StartNew(UpdateFitWidth);
        }


        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            //监控 GridViewColumnHeader 单击事件
            AddHandler(GridViewColumnHeader.ClickEvent, new RoutedEventHandler(OnGridViewColumnHeaderClick));

            AddHandler(GridViewColumnHeader.PreviewMouseLeftButtonUpEvent, new RoutedEventHandler(OnGridViewColumnHeaderPreviewMouseLeftButtonUp));

            //刷新筛选
            RefreshFilter();

            //刷新排序
            InitializeSort();
            RefreshSort();
        }

        #region 自动保存 GridViewColumns 样式
        private void OnGridViewColumnHeaderPreviewMouseLeftButtonUp(object sender, RoutedEventArgs e)
        {
            AsyncAutoSaveColumns();
        }

        private int _autoSaveVersion;
        private void AsyncAutoSaveColumns()
        {
            var version = Interlocked.Increment(ref _autoSaveVersion);
            Task.Delay(2000).ContinueWith(task =>
            {
                if (_autoSaveVersion == version)
                    AutoSaveColumns();
            });
        }

        private void AutoSaveColumns()
        {
            if (CheckAccess())
            {
                if (IsInitialized)
                {
                    //var identity = Identity;
                    //if (!string.IsNullOrEmpty(identity))
                    //{

                    //}

                }
            }
            else
            {
                Dispatcher.Invoke(AutoSaveColumns);
            }
        }



        public Guid Identity
        {
            get { return (Guid)GetValue(IdentityProperty); }
            set { SetValue(IdentityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Identity.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IdentityProperty =
            DependencyProperty.Register("Identity", typeof(Guid), typeof(QListView), new PropertyMetadata(null));


        #endregion 自动保存 GridViewColumns 样式

        private int _itemsVersion = 0;

        public int ItemsVersion
        {
            get { return _itemsVersion; }
        }

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Remove:
                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Move:
                    Interlocked.Increment(ref _itemsVersion);
                    break;
                case NotifyCollectionChangedAction.Reset:

                    break;
            }

            RefreshDataInfo();
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);
            RefreshDataInfo();
        }

        private void RefreshDataInfo()
        {
            var itemsCount = Items?.Count;
            if (itemsCount.HasValue)
            {
                var selectedCount = SelectedItems.Count;
                SetCurrentValue(DataInfoProperty, string.Join(" / ", selectedCount, itemsCount));
            }
            else
            {
                SetCurrentValue(DataInfoProperty, string.Empty);
            }
        }

        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            Interlocked.Increment(ref _itemsVersion);
            base.OnItemsSourceChanged(oldValue, newValue);
        }

        #region 筛选
        public virtual void RefreshFilter()
        {
            if (IsInitialized)
            {
                var filterConditions = GetFilterConditions().ToArray();

                if (filterConditions?.Length > 0)
                {
                    Items.Filter = item =>
                    {
                        if (!OnFilter(item))
                        {
                            return false;
                        }

                        foreach (var condition in filterConditions)
                        {
                            if (!condition.Match(item))
                            {
                                return false;
                            }
                        }
                        return true;
                    };
                }
                else
                {
                    Items.Filter = OnFilter;
                }
            }
        }

        private IEnumerable<IFilterCondition> GetFilterConditions()
        {
            var gridView = View as GridView;
            if (gridView != null)
            {
                foreach (QGridViewColumn column in gridView.Columns)
                {
                    var filter = column.Filter;
                    if (filter != null && filter.IsSelected)
                    {
                        yield return filter;
                    }
                }
            }
        }

        protected bool OnFilter(object item)
        {
            var filter = Filter;
            if (filter != null)
            {
                var e = new QFilterEventArgs(item);
                filter.Invoke(e);
                return e.Accepted;
            }
            else
            {
                return true;
            }
        }

        public Action<QFilterEventArgs> Filter
        {
            get { return (Action<QFilterEventArgs>)GetValue(FilterProperty); }
            set { SetValue(FilterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Filter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FilterProperty =
            DependencyProperty.Register("Filter", typeof(Action<QFilterEventArgs>), typeof(QListView), new PropertyMetadata(null, OnFilterChanged));

        private static void OnFilterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((QListView)d).RefreshFilter();
        }


        #endregion 筛选

        #region 适配宽度

        private int _batchNumberByFitWidth = 0;
        protected void UpdateFitWidth()
        {
            if (CheckAccess())
            {
                var gridView = View as GridView;
                var actionWidth = ActualWidth;
                if (gridView != null && actionWidth > 0)
                {
                    var fitItems = new List<GridViewColumnFitItem>();
                    double fitTotalWidth = 0, totalWidth = 0;
                    foreach (QGridViewColumn column in gridView.Columns)
                    {
                        var width = column.FitWidth;
                        if (double.IsNaN(width))
                        {
                            width = column.Width;
                            if (double.IsNaN(width))
                            {
                                width = column.ActualWidth;
                            }
                        }
                        else
                        {
                            fitTotalWidth += width;
                            fitItems.Add(new GridViewColumnFitItem(column, width));
                        }
                        totalWidth += width;
                    }

                    //增加滚动条大小
                    totalWidth += 20;
                    if (totalWidth < actionWidth)
                    {
                        var ratio = Math.Floor((actionWidth - totalWidth + fitTotalWidth) / fitTotalWidth * 10000) / 10000;
                        foreach (var item in fitItems)
                        {
                            item.Column.Width = Math.Round(item.FitWidth * ratio);
                        }
                    }
                    else
                    {
                        foreach (var item in fitItems)
                        {
                            item.Column.Width = item.FitWidth;
                        }
                    }
                }
            }
            else
            {
                var batchNumber = Interlocked.Increment(ref _batchNumberByFitWidth);
                Task.Delay(100).Wait();
                if (batchNumber == _batchNumberByFitWidth)
                {
                    Dispatcher.Invoke(UpdateFitWidth);
                }
            }
        }

        class GridViewColumnFitItem
        {
            public GridViewColumnFitItem(QGridViewColumn column, double fitWidth)
            {
                Column = column;
                FitWidth = fitWidth;
            }

            public QGridViewColumn Column { get; }

            public double FitWidth { get; }
        }
        #endregion 适配宽度

        #region 排序
        private List<QGridViewColumn> _sortColmuns = new List<QGridViewColumn>();
        private int _batchNumberBySort;
        protected void RefreshSort()
        {
            if (CheckAccess())
            {
                if (IsInitialized)
                {
                    var sortDescriptions = new List<SortDescription>();
                    for (int i = 0; i < _sortColmuns.Count; i++)
                    {
                        var column = _sortColmuns[_sortColmuns.Count - i - 1];
                        sortDescriptions.AddRange(AddColumnSortFields(column));
                    }

                    sortDescriptions.AddRange(AddDefaultSortFields());

                    Items.SortDescriptions.Clear();

                    foreach (var sortDescription in sortDescriptions)
                    {
                        Items.SortDescriptions.Add(sortDescription);
                    }
                }
            }
            else
            {
                Dispatcher.Invoke(RefreshSort);
            }
        }

        private void AsyncRefreshSort()
        {
            var batchNumber = Interlocked.Increment(ref _batchNumberBySort);
            Task.Delay(100).ContinueWith((task) =>
            {
                if (batchNumber == _batchNumberBySort)
                {
                    RefreshSort();
                }
            });

        }

        private void InitializeSort()
        {
            GridView gridView = View as GridView;
            if (gridView != null)
            {
                List<QGridViewColumn> columns = new List<QGridViewColumn>();
                string sortFileds;
                ListSortDirection? direction;
                foreach (QGridViewColumn column in gridView.Columns)
                {
                    direction = column.Direction;
                    if (direction.HasValue)
                    {
                        sortFileds = column.SortFields;
                        if (!string.IsNullOrWhiteSpace(sortFileds))
                        {
                            columns.Add(column);
                        }
                    }
                }

                if (columns.Count > 0)
                {
                    _sortColmuns.Clear();
                    _sortColmuns.AddRange(columns);
                }
            }
        }

        private static IEnumerable<SortDescription> AddColumnSortFields(QGridViewColumn column)
        {
            string sortFields = column.SortFields;
            ListSortDirection? direction = column.Direction;
            if (!string.IsNullOrEmpty(sortFields) && direction.HasValue)
            {
                var fields = sortFields.Split(',');
                for (int i = 0; i < fields.Length; i++)
                {
                    if (!string.IsNullOrEmpty(fields[i]))
                    {
                        yield return new SortDescription(fields[i], direction.Value);
                    }
                }
            }
        }

        private IEnumerable<SortDescription> AddDefaultSortFields()
        {
            string sortFields = SortFields;
            if (!string.IsNullOrEmpty(sortFields))
            {
                var fieldList = sortFields.Split(',');
                for (int i = 0; i < fieldList.Length; i++)
                {
                    if (!string.IsNullOrEmpty(fieldList[i]))
                    {
                        var direction = ListSortDirection.Ascending;
                        var datas = fieldList[i].Split(' ');
                        if (datas.Length > 1)
                        {
                            switch (datas[1].ToUpper())
                            {
                                case "D":
                                case "DESC":
                                    direction = ListSortDirection.Descending;
                                    break;
                                default:
                                    break;
                            }
                        }
                        yield return new SortDescription(datas[0], direction);
                    }
                }
            }
        }

        protected virtual void OnGridViewColumnHeaderClick(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader header = e.OriginalSource as GridViewColumnHeader;
            if (header != null && header.Column != null)
            {
                var column = header.Column as QGridViewColumn;
                var sortField = column.SortFields;
                if (sortField != null)
                {
                    var direction = column.Direction;
                    switch (direction)
                    {
                        case ListSortDirection.Ascending:
                            direction = null;
                            break;
                        case ListSortDirection.Descending:
                            direction = ListSortDirection.Ascending;
                            break;
                        default:
                            direction = ListSortDirection.Descending;
                            break;
                    }

                    column.Direction = direction;
                    _sortColmuns.Remove(column);
                    if (direction.HasValue)
                    {
                        _sortColmuns.Add(column);
                    }

                    AsyncRefreshSort();
                }
            }
        }

        public string SortFields
        {
            get { return (string)GetValue(SortFieldsProperty); }
            set { SetValue(SortFieldsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SortFields.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SortFieldsProperty =
            DependencyProperty.Register("SortFields", typeof(string), typeof(QListView), new PropertyMetadata(null, OnSortFieldsChanged));

        private static void OnSortFieldsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as QListView)?.AsyncRefreshSort();
        }
        #endregion 排序



        public string DataInfo
        {
            get { return (string)GetValue(DataInfoProperty); }
        }

        // Using a DependencyProperty as the backing store for DataInfo.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataInfoProperty =
            DependencyProperty.Register("DataInfo", typeof(string), typeof(QListView), new PropertyMetadata(null));


    }
}
