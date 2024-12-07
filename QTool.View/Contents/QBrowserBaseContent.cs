using CefSharp;
using System;
using System.Linq;
using System.Windows;
using QTool.View.Models;
using System.Collections.ObjectModel;
using QTool.View.Controls;
using System.Collections.Generic;
using System.Collections.Specialized;
using QTool.View.DAL;

namespace QTool.View.Contents
{
    public abstract class QBrowserBaseContent : BaseContent
    {
        public QBrowserBaseContent(QModule module, int shopId, string uriString)
            : base(module)
        {
            ShopId = shopId;
            RefreshUserId();
            UriString = uriString;
            LifeSpanHandler = new QLifeSpanHandler(this);
            RequestHandler = new QRequestHandler(this);
            DownloadHandler = new QDownloadHandler();
            MenuHandler = new QMenuHandler(this);
            RecreateContent();
        }

        public override string HeaderTemplate => "BrowserHeader";

        public abstract string Icon { get; }

        public void RefreshUserId()
        {
            if (ShopId > 0 && AccountDAL.Current.TryGetShopUserId(ShopId, out string userId))
            {
                UserId = userId;
            }
            else
            {
                UserId = null;
            }
        }

        public int ShopId { get; }

        public string UserId { get; private set; }

        public string UriString { get; }

        public string DefaultUri
        {
            get;
            private set;
        }

        public ILifeSpanHandler LifeSpanHandler { get; }

        public IRequestHandler RequestHandler { get; }

        public IDownloadHandler DownloadHandler { get; }

        public IContextMenuHandler MenuHandler { get; }

        public string GetCachePath()
        {
            if (string.IsNullOrWhiteSpace(UserId))
            {
                return GlobalHelper.CreatePath("Cache", Guid.NewGuid().ToString("N"));
            }
            else
            {
                return GlobalHelper.CreatePath("Cache", QContext.Current.AccountId.ToFolderName(), UserId);
            }

        }

        protected abstract QWebBrowserBase CreateContent(string address);

        #region Added
        private EventHandler<ShopAddedEventArgs> _added;
        public event EventHandler<ShopAddedEventArgs> Added
        {
            add { _added += value; }
            remove { _added -= value; }
        }

        internal void OnAdded(ShopAddedEventArgs e)
        {
            _added?.Invoke(this, e);
        }
        #endregion Added

        #region Tabs Property
        private ObservableCollection<QWebBrowserBase> _tabs;
        /// <summary>
        /// Tabs
        /// </summary>
        public ObservableCollection<QWebBrowserBase> Tabs
        {
            get
            {
                return _tabs;
            }
            set
            {
                if (_tabs != value)
                {
                    _tabs = value;
                    OnPropertyChanged(nameof(Tabs));
                }
            }
        }
        #endregion Tabs Property

        protected virtual string GetDefaultUri()
        {
            return UriString;
        }

        public void RecreateContent()
        {
            IsWaiting = true;
            AsyncHelper.Exec(() =>
            {
                RefreshUserId();
                return GetDefaultUri();
            }, result =>
            {
                IsWaiting = false;
                DefaultUri = result.Result;
                Content = CreateContent(DefaultUri);
                Tabs = new ObservableCollection<QWebBrowserBase>{
                    Content
                };
            });
        }

        #region Content Property
        private QWebBrowserBase _tabPosition;
        private QWebBrowserBase _content;
        /// <summary>
        /// 当前内容
        /// </summary>
        public QWebBrowserBase Content
        {
            get
            {
                return _content;
            }
            set
            {
                if (_content != value)
                {
                    _content?.Mark();
                    _tabPosition = _content = value;
                    OnPropertyChanged(nameof(Content));
                }
            }
        }
        #endregion Content Property

        #region LatelyContent Property
        private QWebBrowserBase _latelyContent;
        /// <summary>
        /// 延迟加载选项卡
        /// </summary>
        public QWebBrowserBase LatelyContent
        {
            get
            {
                return _latelyContent;
            }
            set
            {
                if (_latelyContent != value)
                {
                    _latelyContent = value;
                    OnPropertyChanged(nameof(LatelyContent));
                }
            }
        }
        #endregion LatelyContent Property

        public IWebBrowser AddTab(string targetUrl, WebBrowserTabOpenWith openWith)
        {
            if (Application.Current.CheckAccess())
            {
                QWebBrowserBase tab = null;
                if (tab == null)
                {
                    tab = CreateContent(targetUrl);
                    int index = -1;
                    if (_tabPosition != null)
                    {
                        index = Tabs.IndexOf(_tabPosition);
                    }

                    if (index == -1 && Content != null)
                    {
                        index = Tabs.IndexOf(Content);
                    }

                    if (index > 0 && index < Tabs.Count)
                        Tabs.Insert(index + 1, tab);
                    else
                        Tabs.Add(tab);
                }

                if (openWith == WebBrowserTabOpenWith.Background)
                {
                    _tabPosition = LatelyContent = tab;
                }
                else
                {
                    Content = tab;
                }
                return tab.Browser.WebBrowser;
            }
            else
            {
                return (IWebBrowser)Application.Current.Dispatcher.Invoke(new Func<string, WebBrowserTabOpenWith, IWebBrowser>(AddTab), targetUrl, openWith);
            }
        }

        public bool CloseTab(QWebBrowserBase tab)
        {
            if (Application.Current.CheckAccess())
            {
                var index = Tabs.IndexOf(tab);
                if (index != -1)
                {
                    Tabs.RemoveAt(index);

                    if (Tabs.Count == 0)
                    {
                        RecreateContent();
                    }
                    else if (Content == null || Content == tab)
                    {
                        if (index < Tabs.Count)
                        {
                            Content = Tabs[index];
                        }
                        else
                        {
                            Content = Tabs.LastOrDefault();
                        }
                    }
                    return true;
                }
                tab.DisposeWebBrowser();
                return false;
            }
            else
            {
                return (bool)Application.Current.Dispatcher.Invoke(new Func<QWebBrowserBase, bool>(CloseTab), tab);
            }
        }

        public void CloseOtherTabs(QWebBrowserBase currentTab)
        {
            var tabs = Tabs.ToArray();
            foreach (var tab in tabs)
            {
                if (tab != currentTab)
                    CloseTab(tab);
            }
        }

        public void CloseOffsideTabs(QWebBrowserBase currentTab)
        {
            var index = Tabs.IndexOf(currentTab);
            var tabs = Tabs.Skip(index + 1).ToArray();
            foreach (var tab in tabs)
            {
                CloseTab(tab);
            }
        }

        public static void RemoveShop(IShop shop)
        {
            if (QContext.Current.TryGetCookieManager(shop.ShopId, out QCookieManager cookieManager))
            {
                switch (shop.Platform)
                {
                    case QPlatform.AliExpress:
                        AeBrowserContent.RemoveShop(cookieManager.UserId);
                        break;
                    case QPlatform.Temu:
                        TemuBrowserContent.RemoveShop(cookieManager.UserId);
                        break;
                    default:
                        break;
                }
            }
        }

        public virtual void SetRequestHeaders(NameValueCollection headers)
        {

        }
    }
}
