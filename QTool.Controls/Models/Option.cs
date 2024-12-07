using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTool.Controls.Models
{
    public class Option<TData> : INotifyPropertyChanged
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

        #region IsSelected Property
        private bool _isSelected;
        /// <summary>
        /// 
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
                    _selectedChanged?.Invoke(this, new SelectedChangedEventArgs<Option<TData>>(this, IsSelected));
                }
            }
        }
        #endregion IsSelected Property

        #region SelectedChanged
        private EventHandler<SelectedChangedEventArgs<Option<TData>>> _selectedChanged;

        public event EventHandler<SelectedChangedEventArgs<Option<TData>>> SelectedChanged
        {
            add { _selectedChanged += value; }
            remove { _selectedChanged -= value; }
        }
        #endregion SelectedChanged

        public TData Data { get; }

        public Option(TData data, bool isSelected = false)
        {
            Data = data;
            IsSelected = isSelected;
        }
    }
}