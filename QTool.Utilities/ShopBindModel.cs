using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTool
{
    public class ShopBindModel : ShopModel
    {
        #region ShopId Property
        private int? _shopId;
        /// <summary>
        /// 
        /// </summary>
        public int? ShopId
        {
            get
            {
                return _shopId;
            }
            set
            {
                if (_shopId != value)
                {
                    _shopId = value;
                    OnPropertyChanged(nameof(ShopId));
                    OnPropertyChanged(nameof(IsAdded));
                }
            }
        }
        #endregion ShopId Property

        #region DisplayName Property
        private string _displayName;
        /// <summary>
        /// 
        /// </summary>
        public string DisplayName
        {
            get
            {
                return _displayName;
            }
            set
            {
                if (_displayName != value)
                {
                    _displayName = value;
                    OnPropertyChanged(nameof(DisplayName));
                }
            }
        }
        #endregion DisplayName Property



        public bool IsAdded { get { return ShopId > 0; } }

        public override string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case nameof(ShopIdentity):
                        if (string.IsNullOrEmpty(ShopIdentity))
                        {
                            return "店铺账号不能为空。";
                        }
                        break;
                    case nameof(DisplayName):
                        if (string.IsNullOrWhiteSpace(DisplayName))
                        {
                            return "显示名不能为空。";
                        }
                        break;
                    default:
                        break;
                }

                return string.Empty;
            }
        }

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
            set
            {
                if (_message != value)
                {
                    _message = value;
                    OnPropertyChanged(nameof(Message));
                }
            }
        }
        #endregion Message Property
    }
}
