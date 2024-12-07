using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QTool
{
    public abstract class ModelBase : IDataErrorInfo, INotifyPropertyChanged
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

        public virtual string this[string columnName]
        {
            get
            {
                return string.Empty;
            }
        }

        public string Error
        {
            get
            {
                var properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty);
                List<string> errors = new List<string>();
                foreach (var property in properties)
                {
                    if (property.CanRead)
                    {
                        var error = this[property.Name];
                        if (!string.IsNullOrEmpty(error))
                        {
                            errors.Add(error);
                        }
                    }
                }
                return string.Join(Environment.NewLine, errors);
            }
        }
    }
}
