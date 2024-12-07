using Newtonsoft.Json.Linq;
using NPOI.HSSF.Record.Chart;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace QTool.Controls.Models
{

    public class FilterOption : IFilterOption, INotifyPropertyChanged
    {
        public FilterOption(QGridViewColumn column, object value)
        {
            Column = column ?? throw new ArgumentNullException(nameof(column));
            Value = value;
        }

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

        public object Value { get; }

        public QGridViewColumn Column { get; }

        #region DataCount Property
        private int _dataCount;
        /// <summary>
        /// 数量
        /// </summary>
        public int DataCount
        {
            get
            {
                return _dataCount;
            }
            set
            {
                if (_dataCount != value)
                {
                    _dataCount = value;
                    OnPropertyChanged(nameof(DataCount));
                }
            }
        }
        #endregion DataCount Property

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
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                    OnSelectedChanged(new SelectedChangedEventArgs<QGridViewColumn>(Column, IsSelected));
                }
            }
        }

        #endregion IsSelected Property

        #region SelectedChanged
        private event EventHandler<SelectedChangedEventArgs<QGridViewColumn>> _selectedChanged;
        public event EventHandler<SelectedChangedEventArgs<QGridViewColumn>> SelectedChanged
        {
            add
            {
                _selectedChanged += value;
            }
            remove
            {
                _selectedChanged -= value;
            }
        }

        protected virtual void OnSelectedChanged(SelectedChangedEventArgs<QGridViewColumn> args)
        {
            _selectedChanged?.Invoke(this, args);
            args.Data.FindParent<QListView>()?.RefreshFilter();
        }
        #endregion SelectedChanged


        public virtual bool Match(object value)
        {
            if (Value == null)
            {
                return value == null;
            }
            else
            {
                return CompareHelper.CompareTo(Value, value) == 0;
            }
        }

        public int CompareTo(object otherObj)
        {
            var other = otherObj as FilterOption;

            var value = Value as IComparable;
            var otherValue = other.Value as IComparable;
            if (value == null)
            {
                if (otherValue == null)
                {
                    return 0;
                }
                else
                {
                    return 1;
                }
            }
            else
            {
                if (otherValue == null)
                {
                    return -1;
                }
                else
                {
                    return value.CompareTo(otherValue);
                }
            }
        }
    }
}
