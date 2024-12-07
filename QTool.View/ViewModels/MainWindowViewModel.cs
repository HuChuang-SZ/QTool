using QTool.Controls;
using QTool.View.Contents;
using QTool.View.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

namespace QTool.View.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private MainWindowViewModel()
        {

        }

        public static MainWindowViewModel Current { get; } = new MainWindowViewModel();

        public bool IsInitialized { get; private set; } = false;

        public void Initialized()
        {
            if (CurrentShop == null || !Shops.Contains(CurrentShop))
            {
                CurrentShop = Shops.FirstOrDefault();
            }
            IsInitialized = true;

            UpdateContent();
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

        #region IsWaiting Property
        private bool _isWaiting;
        /// <summary>
        /// 是否等待中
        /// </summary>
        public bool IsWaiting
        {
            get
            {
                return _isWaiting;
            }
            set
            {
                if (_isWaiting != value)
                {
                    _isWaiting = value;
                    OnPropertyChanged(nameof(IsWaiting));
                }
            }
        }
        #endregion IsWaiting Property

        #region WaitMsg Property
        private string _waitMsg;
        /// <summary>
        /// 等待消息
        /// </summary>
        public string WaitMsg
        {
            get
            {
                return _waitMsg;
            }
            set
            {
                if (_waitMsg != value)
                {
                    _waitMsg = value;
                    OnPropertyChanged(nameof(WaitMsg));
                }
            }
        }
        #endregion WaitMsg Property

        #region CurrentModule Property
        private QModuleInfo _currentModule = QModuleInfo.Find();
        /// <summary>
        /// 当前模块
        /// </summary>
        public QModuleInfo CurrentModule
        {
            get
            {
                return _currentModule;
            }
            set
            {
                if (_currentModule != value)
                {
                    _currentModule = value;
                    OnPropertyChanged(nameof(CurrentModule));
                    UpdateShops();
                    UpdateContent();
                }
            }
        }

        private void UpdateShops()
        {
            OnPropertyChanged(nameof(Shops));
            if (CurrentModule != null && (CurrentShop == null || CurrentShop.Platform != CurrentModule.Platform))
            {
                CurrentShop = Shops.FirstOrDefault();
            }
        }
        #endregion CurrentModule Property

        public ReadOnlyCollection<LoginShop> Shops
        {
            get
            {
                return QContext.Current.Shops;
            }
        }

        #region CurrentShop Property
        private LoginShop _currentShop;

        /// <summary>
        /// 当前店铺
        /// </summary>
        public LoginShop CurrentShop
        {
            get
            {
                return _currentShop;
            }
            set
            {
                if (_currentShop != value)
                {
                    _currentShop = value;
                    OnPropertyChanged(nameof(CurrentShop));
                    UpdateContent(true);
                }
            }
        }
        #endregion CurrentShop Property

        #region CurrentContent Property
        private IBaseContent _currentContent;
        /// <summary>
        /// 当前内容
        /// </summary>
        public IBaseContent CurrentContent
        {
            get
            {
                return _currentContent;
            }
            private set
            {
                if (_currentContent != value)
                {
                    _currentContent = value;
                    OnPropertyChanged(nameof(CurrentContent));
                }
            }
        }


        private void UpdateContent(bool autoLogin = false)
        {
            if (IsInitialized)
            {
                if (CurrentShop == null)
                {
                    CurrentContent = null;
                }
                else
                {
                    if (autoLogin && CurrentShop.Error is NotLoggedException)
                    {
                        //QFactory.Login(CurrentShop);
                    }
                    CurrentContent = BaseContent.Create(CurrentModule, CurrentShop.ShopId);
                }
            }
        }

        public void ShopClearUp(LoginShop shop)
        {
            if (Application.Current.CheckAccess())
            {
                QBrowserBaseContent.RemoveShop(shop);

                if (CurrentShop == shop)
                {
                    UpdateContent();
                }
            }
            else
            {
                Application.Current.Dispatcher.Invoke(new Action<LoginShop>(ShopClearUp), shop);
            }
        }
        #endregion CurrentContent Property

    }
}
