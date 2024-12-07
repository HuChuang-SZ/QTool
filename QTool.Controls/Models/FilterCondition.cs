
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Windows.Controls;
using NPOI.SS.Formula.Functions;
using System.Diagnostics;
using System.Collections.ObjectModel;
using NPOI.HSSF.Record.Chart;
using System.Windows.Documents;
using System.Windows;

namespace QTool.Controls.Models
{
    public class FilterCondition : IFilterCondition, INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        [NonSerialized]
        private PropertyChangedEventHandler _propertyChanged;
        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                _propertyChanged += value;
            }
            remove
            {
                _propertyChanged -= value;
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            _propertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion INotifyPropertyChanged

        public FilterType FilterType { get; }

        public int? FilterRange { get; }

        public object GetPropertyValue(object o)
        {
            return _propertyAccessor.GetValue(o);
        }

        #region Options Property
        private ReadOnlyCollection<IFilterOption> _options;
        /// <summary>
        /// 
        /// </summary>
        public ReadOnlyCollection<IFilterOption> Options
        {
            get
            {
                return _options;
            }
            private set
            {
                if (_options != value)
                {

                    if (Options != null)
                    {
                        foreach (var option in Options)
                        {
                            option.SelectedChanged -= Option_SelectedChanged;
                        }
                    }
                    _options = value;
                    OnPropertyChanged(nameof(Options));

                    if (Options != null)
                    {
                        foreach (var option in Options)
                        {
                            option.SelectedChanged += Option_SelectedChanged;
                        }
                    }
                }
            }
        }

        private void Option_SelectedChanged(object sender, SelectedChangedEventArgs<QGridViewColumn> e)
        {
            IsSelected = Options.Any(o => o.IsSelected);
        }
        #endregion Options Property

        #region IsSelected Property
        private bool _isSelected;

        /// <summary>
        /// 选择项
        /// </summary>
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            private set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }

        #endregion IsSelected Property

        #region IsGenerating Property
        private bool _isGenerating;
        /// <summary>
        /// 
        /// </summary>
        public bool IsGenerating
        {
            get
            {
                return _isGenerating;
            }
            private set
            {
                if (_isGenerating != value)
                {
                    _isGenerating = value;
                    OnPropertyChanged(nameof(IsGenerating));
                }
            }
        }
        #endregion IsGenerating Property

        #region Message Property
        private string _message;
        /// <summary>
        /// 
        /// </summary>
        public string Message
        {
            get
            {
                return _message;
            }
            private set
            {
                if (_message != value)
                {
                    _message = value;
                    OnPropertyChanged(nameof(Message));
                }
            }
        }
        #endregion Message Property

        private readonly PropertyAccessor _propertyAccessor;
        public FilterCondition(string propertyPath, FilterType filterType, int? filterRange)
        {
            _propertyAccessor = new PropertyAccessor(propertyPath);
            FilterType = filterType;
            FilterRange = filterRange;
        }

        private int _itemsVersion;
        public void CreateOptions(QGridViewColumn column)
        {
            var listView = column.FindParent<QListView>();
            if (listView != null)
            {
                int itemsVersion = listView.ItemsVersion;
                object[] items;
                if (listView.ItemsSource == null)
                {
                    items = Array.Empty<object>();
                }
                else if (listView.ItemsSource is ICollectionView)
                {
                    items = ((ICollectionView)listView.ItemsSource).SourceCollection.Cast<object>().ToArray();
                }
                else
                {
                    items = listView.ItemsSource.Cast<object>().ToArray();
                }
                if (itemsVersion != _itemsVersion)
                {
                    Stopwatch stopwatch = Stopwatch.StartNew();
                    _itemsVersion = itemsVersion;
                    Options = null;
                    IsGenerating = true;
                    Message = "加载中...";
                    AsyncHelper.Exec(() =>
                    {
                        var options = new SortedSet<IFilterOption>();
                        var selectedOptions = Options?.Where(o => o.IsSelected).ToArray();
                        if (selectedOptions?.Length > 0)
                        {
                            foreach (var item in selectedOptions)
                            {
                                item.DataCount = 0;
                                options.Add(item);
                            }
                        }
                        foreach (var item in items)
                        {
                            var value = GetPropertyValue(item);
                            if (FilterType == FilterType.Array)
                            {
                                var array = value as IEnumerable;
                                int index = 0;
                                if (array != null)
                                {
                                    foreach (var vItem in array)
                                    {
                                        index++;
                                        TryCreateOption(options, column, vItem);
                                    }
                                }
                                if (index == 0)
                                {
                                    TryCreateOption(options, column, null);
                                }
                            }
                            else
                            {
                                TryCreateOption(options, column, value);
                            }
                        }
                        return options;

                    }, result =>
                    {
                        if (itemsVersion == listView.ItemsVersion)
                        {
                            stopwatch.Stop();
                            if (result.HasError)
                            {
                                Message = result.Error.GetInnerException().Message;
                                _itemsVersion -= 1;
                            }
                            else
                            {
                                IsGenerating = false;
                                Message = $"耗时：{stopwatch.Elapsed.TotalSeconds:0.##} 秒";
                                Options = new ReadOnlyCollection<IFilterOption>(result.Result.ToArray());
                            }
                        }
                    });
                }
            }
        }

        private void TryCreateOption(SortedSet<IFilterOption> options, QGridViewColumn column, object value)
        {
            IFilterOption option;
            if (FilterType == FilterType.Array && value != null)
            {
                var array = new object[] { value };
                option = options.FirstOrDefault(o => o.Match(array));
            }
            else
            {
                option = options.FirstOrDefault(o => o.Match(value));
            }
            if (option == null)
            {
                option = CreateOption(column, value);
                options.Add(option);
            }
            option.DataCount += 1;
        }

        private IFilterOption CreateOption(QGridViewColumn column, object value)
        {
            switch (FilterType)
            {
                case FilterType.Number:
                    return new FilterNumberOption(column, new FilterNumberValue(value, FilterRange ?? 10));
                case FilterType.Array:
                    return new FilterArrayOption(column, value);
                default:
                    return new FilterOption(column, value);
            }
        }

        public bool Match(object item)
        {
            if (IsSelected)
            {
                var value = GetPropertyValue(item);
                return Options.Any(option => option.IsSelected && option.Match(value));
            }
            else
            {
                return true;
            }
        }
    }
}
