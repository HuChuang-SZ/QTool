using LiteDB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;

namespace QTool
{
    public class LoginShop : IShop, INotifyPropertyChanged
    {
        [BsonId]
        public int ShopId
        {
            get;
            set;
        }

        public QPlatform Platform
        {
            get;
            set;
        }

        public string ShopIdentity
        {
            get;
            set;
        }

        public string DisplayName
        {
            get;
            set;
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

        public string FullName
        {
            get
            {
                if (HasError)
                    return $"{DisplayName}({ShopIdentity})(未登录)";
                else
                    return $"{DisplayName}({ShopIdentity})({Unread})";
            }
        }

        public DateTime UnreadTime { get; set; }


        #region Unread Property
        private int _unread;
        /// <summary>
        /// 未读消息
        /// </summary>
        public int Unread
        {
            get
            {
                return _unread;
            }
            set
            {
                if (_unread != value)
                {
                    _unread = value;
                    OnPropertyChanged(nameof(Unread));
                    OnPropertyChanged(nameof(FullName));
                }
            }
        }
        #endregion Unread Property

        #region Error Property
        private Exception _error;
        /// <summary>
        /// 
        /// </summary>
        public Exception Error
        {
            get
            {
                return _error;
            }
            set
            {
                if (_error != value)
                {
                    _error = value;
                    OnPropertyChanged(nameof(Error));
                    OnPropertyChanged(nameof(HasError));
                }
            }
        }

        public bool HasError
        {
            get
            {
                return Error != null;
            }
        }

        #endregion Error Property
    }
}
