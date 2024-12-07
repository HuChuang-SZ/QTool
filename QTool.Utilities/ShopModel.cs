using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTool
{
    public class ShopModel : ModelBase
    {
        #region Platform Property
        private QPlatform _platform;
        /// <summary>
        /// 
        /// </summary>
        public QPlatform Platform
        {
            get
            {
                return _platform;
            }
            set
            {
                if (_platform != value)
                {
                    _platform = value;
                    OnPropertyChanged(nameof(Platform));
                }
            }
        }
        #endregion Platform Property

        #region ShopIdentity Property
        private string _shopIdentity;
        /// <summary>
        /// 店铺标识
        /// </summary>
        public string ShopIdentity
        {
            get
            {
                return _shopIdentity;
            }
            set
            {
                if (_shopIdentity != value)
                {
                    _shopIdentity = value;
                    OnPropertyChanged(nameof(ShopIdentity));
                }
            }
        }
        #endregion ShopIdentity Property

        public override string this[string columnName]
        {
            get
            {

                return base[columnName];
            }
        }
    }
}
