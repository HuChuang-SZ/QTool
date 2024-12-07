using CefSharp.Wpf;
using CefSharp;
using QTool.View.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using QTool.Controls;
using QTool.View.Contents;
using CefSharp.Wpf.Experimental;
using QTool.Api.Ae;
using QTool.View.DAL;

namespace QTool.View.Models
{
    public abstract class QWebBrowserBase : INotifyPropertyChanged
    {
        protected const int IntervalMilliseconds = 100;
        protected ShopLoginUser _currentLoginUser = new ShopLoginUser();
        public QWebBrowserBase(QBrowserBaseContent owner, string address)
        {
            Owner = owner;
            Address = address;
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

        #region Address Property
        private string _address;
        /// <summary>
        /// 地址
        /// </summary>
        public string Address
        {
            get
            {
                return _address;
            }
            private set
            {
                if (_address != value)
                {
                    _address = value;
                    OnPropertyChanged(nameof(Address));
                }
            }
        }
        #endregion Address Property

        #region Title Property
        private string _title = "待加载";
        /// <summary>
        /// 标题
        /// </summary>
        public string Title
        {
            get
            {
                return _title;
            }
            private set
            {
                if (_title != value)
                {
                    _title = value;
                    OnPropertyChanged(nameof(Title));
                }
            }
        }
        #endregion Title Property

        public QBrowserBaseContent Owner { get; }

        #region Browser Property
        private ChromiumWebBrowser _browser;
        /// <summary>
        /// describe
        /// </summary>
        public ChromiumWebBrowser Browser
        {
            get
            {
                lock (this)
                {
                    if (_browser == null)
                    {
                        _browser = CreateWebBrowser(Address);
                    }
                }
                Mark();
                return _browser;
            }
            set
            {
                if (_browser != value)
                {
                    _browser = value;
                    OnPropertyChanged(nameof(Browser));
                }
            }
        }

        public void Mark()
        {
            _lastAccessTime = DateTime.Now;
            QWebBrowserDispatch.Mark(this);
        }
        #endregion Browser Property


        private DateTime _lastAccessTime;
        public double LastAccessMinutes()
        {
            return (DateTime.Now - _lastAccessTime).TotalMinutes;
        }

        protected ShopLogin GetShopLogin(bool ingoreNull = true)
        {
            return Owner.ShopId > 0 ? QContext.Current.GetShopLogin(Owner.ShopId, ingoreNull) : null;
        }


        private ChromiumWebBrowser CreateWebBrowser(string address)
        {
            if (Application.Current.CheckAccess() == true)
            {
                //独立Cookie
                var context = new RequestContext(new RequestContextSettings()
                {
                    CachePath = Owner.GetCachePath(),
                    PersistSessionCookies = true,
                    PersistUserPreferences = true,
                    AcceptLanguageList = "zh-CN,zh;q=0.9,en;q=0.8,en-GB;q=0.7,en-US;q=0.6",
                });

                var webBrowser = new ChromiumWebBrowser();
                webBrowser.WpfKeyboardHandler = new WpfImeKeyboardHandler(webBrowser);
                webBrowser.RequestContext = context;
                webBrowser.IsBrowserInitializedChanged += WebBrowser_IsBrowserInitializedChanged;
                webBrowser.FrameLoadEnd += WebBrowser_FrameLoadEnd;
                webBrowser.LoadingStateChanged += WebBrowser_LoadingStateChanged;
                ChromiumWebBrowserExt.SetOwner(webBrowser, this);
                //弹窗处理
                webBrowser.LifeSpanHandler = Owner.LifeSpanHandler;
                webBrowser.RequestHandler = Owner.RequestHandler;
                webBrowser.MenuHandler = Owner.MenuHandler;
                webBrowser.DownloadHandler = Owner.DownloadHandler;

                webBrowser.JavascriptMessageReceived += WebBrowser_JavascriptMessageReceived;
                webBrowser.Address = address;

                return webBrowser;
            }
            else
            {
                return Application.Current.Dispatcher.Invoke(() => CreateWebBrowser(address));
            }
        }

        private void WebBrowser_JavascriptMessageReceived(object sender, JavascriptMessageReceivedEventArgs e)
        {
#if DEBUG123
            QMessageBox.Show(JsonConvert.SerializeObject(e.Message));
#endif
            if (e.Message is IDictionary<string, object> msg)
            {
                foreach (var item in msg)
                {
                    if (item.Value is string val)
                    {
                        switch (item.Key)
                        {
                            case "Account":
                                _currentLoginUser.Account = val;
                                break;
                            case "Password":
                                _currentLoginUser.Password = val;
                                break;
                        }
                    }
                }
            }
        }

        private void WebBrowser_LoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            if (_browser == sender)
            {
                if (!e.IsLoading)
                {
                    _browser.Dispatcher.Invoke(() =>
                    {
                        Title = _browser.Title;
                        Address = _browser.Address;
                        Owner.WaitMsg = null;
                    });
                }
                else
                {
                    Title = "正在加载...";
                }
            }
        }


        /// <summary>
        /// WebBrowser初始化后加载Cookies
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WebBrowser_IsBrowserInitializedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                var webBrowser = (ChromiumWebBrowser)sender;
                var cookies = GetShopLogin()?.CookieManager.Cookies;
                var cookieManager = webBrowser.GetCookieManager();
                if (cookies?.Length > 0)
                {
                    foreach (var cookie in cookies)
                    {
                        cookieManager.SetCookie(DefaultUri, cookie.ConvertToCef());
                    }
                }
                else
                {
                    cookieManager.DeleteCookies();
                }
            }
        }

        public abstract string DefaultUri { get; }


        public void DisposeWebBrowser()
        {
            if (_browser != null && !_browser.IsDisposed)
            {
                _browser.Dispatcher.Invoke(() =>
                {
                    try
                    {
                        var browser = _browser;
                        Browser = null;
                        browser.IsBrowserInitializedChanged -= WebBrowser_IsBrowserInitializedChanged;
                        browser.FrameLoadEnd -= WebBrowser_FrameLoadEnd;
                        ChromiumWebBrowserExt.SetOwner(browser, null);
                        browser.Dispose();
                    }
                    catch { }
                });
            }
        }

        public void Refresh()
        {
            Browser?.Reload();
        }

        public void Close()
        {
            if (Owner != null)
            {
                Owner.CloseTab(this);
            }
        }

        protected int _loginVersion = 0;
        protected bool _isPendingLogin = false;
        /// <summary>
        /// 页面加载后执行方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private async void WebBrowser_FrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            if (e.Frame.IsMain && e.Frame.IsValid)
            {
                var chromiumWebBrowser = sender as ChromiumWebBrowser;
                if (Uri.TryCreate(e.Url, UriKind.Absolute, out Uri uri))
                {
                    switch (GetPageKind(uri))
                    {
                        ///成功登录后执行
                        case PageKind.Home:
                            if (_isPendingLogin)
                            {
                                await CheckLogin(chromiumWebBrowser, Interlocked.Increment(ref _loginVersion));
                            }
                            break;
                        case PageKind.Login:
                            _isPendingLogin = true;
                            Owner.IsWaiting = true;
                            Owner.WaitMsg = "正在登录...";
                            await InputPassword(chromiumWebBrowser);
                            Owner.IsWaiting = false;
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        protected abstract PageKind GetPageKind(Uri uri);

        protected abstract Task InputPassword(ChromiumWebBrowser chromiumWebBrowser);


        /// <summary>
        /// 检查登录状态
        /// </summary>
        /// <param name="webBrowser"></param>
        /// <param name="browser"></param>
        /// <param name="loginVersion"></param>
        /// <returns></returns>
        private async Task CheckLogin(ChromiumWebBrowser webBrowser, int loginVersion, int retryCount = 3)
        {
            await Task.Delay(IntervalMilliseconds);
            if (loginVersion == _loginVersion)
            {
                try
                {
                    if (!webBrowser.IsDisposed)
                    {
                        var cookies = await webBrowser.GetCookieManager().VisitUrlCookiesAsync(DefaultUri, true);
                        await CheckLogin(cookies.Convert().ToArray());
                    }
                }
                catch (Exception ex)
                {
                    if (retryCount > 0)
                    {
                        await CheckLogin(webBrowser, loginVersion, retryCount - 1);
                    }
                    else
                    {
                        LoginFailed(ex);
                    }
                }
            }
        }

        protected abstract Task CheckLogin(QCookie[] cookies);

        private void LoginFailed(Exception ex)
        {
            if (Application.Current.CheckAccess())
            {
                var shop = GetShopLogin(true);
                if (shop != null)
                {
                    QContext.Current.ShopClearUp(shop.Shop);
                }

                if (ex is ApiAeException apiEx && apiEx.Code == ApiErrorCode.NON_CURRENT_STORE)
                {
                    QMessageWindow.Show(ex.Message, "登录失败", MessageBoxButton.OK);
                }
                else
                {
                    QMessageBox.Show(ex.ToString());
                }

                Owner.RecreateContent();
            }
            else
            {
                Application.Current.Dispatcher.BeginInvoke(new Action<Exception>(LoginFailed), ex);
            }
        }

        protected void SaveLogin(QCookieManager cookieManager, string userId, params ShopBindModel[] shops)
        {
            if (cookieManager is null)
            {
                throw new ArgumentNullException(nameof(cookieManager));
            }
            if (shops is null || shops.Length == 0)
            {
                throw new ArgumentNullException(nameof(shops));
            }

            foreach (var shop in shops)
            {
                var loginShop = QContext.Current.GetLoginShop(shop.Platform, shop.ShopIdentity);
                if (loginShop != null)
                {
                    shop.ShopId = loginShop.ShopId;
                    shop.DisplayName = loginShop.DisplayName;
                }
            }

            var loginUser = GetLoginUser(userId, cookieManager.Platform);

            if (Owner.ShopId > 0)
            {
                if (shops.FirstOrDefault(s => s.ShopId == Owner.ShopId) == null)
                {
                    throw new ApiAeException(ApiErrorCode.NON_CURRENT_STORE);
                }
                else
                {
                    if (string.IsNullOrEmpty(userId))
                    {
                        userId = Owner.ShopId.ToString();
                    }

                    if (loginUser != null)
                        AccountDAL.Current.SetShopLoginUser(loginUser);

                    if (QContext.Current.TryGetCookieManager(userId, out QCookieManager manager))
                    {
                        manager.Cookies = cookieManager.Cookies;
                    }
                    else
                    {
                        AccountDAL.Current.SetShopLoginCookie(new ShopLoginCookie() { UserId = userId, Platform = cookieManager.Platform, Cookies = cookieManager.Cookies, ExtDatas = cookieManager.ExtDatas });
                    }
                    AccountDAL.Current.SetShopUserId(userId, shops.Where(s => s.ShopId.HasValue).Select(s => s.ShopId.Value).ToArray());
                }
            }

            Owner.OnAdded(new ShopAddedEventArgs(cookieManager, loginUser, shops));
        }

        private ShopLoginUser GetLoginUser(string userId, QPlatform platform)
        {
            var loginUser = new ShopLoginUser()
            {
                UserId = userId,
                Platform = platform,
            };

            if (!string.IsNullOrEmpty(_currentLoginUser.Account))
                loginUser.Account = _currentLoginUser.Account;

            if (!string.IsNullOrEmpty(_currentLoginUser.Password))
                loginUser.Password = new RSAHelper().Encrypt(_currentLoginUser.Password);

            return loginUser;
        }
    }
}
