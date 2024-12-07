using CefSharp;
using QTool.Controls;
using QTool.View.Contents;
using QTool.View.Models;
using QTool.View.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace QTool.View
{
    /// <summary>
    /// AePage.xaml 的交互逻辑
    /// </summary>
    public partial class AePage : QWindow
    {
        private readonly AePageViewModel _viewModel;

        public AePage(IShop shop, string title, string uriString)
        {
            InitializeComponent();
            DataContext = _viewModel = new AePageViewModel(shop, title, uriString);
            
        }


        public IWebBrowser GetBrowser()
        {
            return _viewModel.Content.Content.Browser.WebBrowser;
        }

        #region 绑定命令
        private void Tab_CanExceute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = GetCurrentTab(e.Parameter) != null;
        }

        private QWebBrowserBase GetCurrentTab(object parameter)
        {
            if (parameter is QWebBrowserBase browser)
            {
                return browser;
            }
            else
            {
                return _viewModel.Content.Content;
            }
        }

        private void RefreshTab_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            GetCurrentTab(e.Parameter)?.Refresh();
        }

        private void CloseTab_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var currentTab = GetCurrentTab(e.Parameter);
            if (currentTab != null)
            {
                currentTab.Owner.CloseTab(currentTab);
            }
        }

        private void CloseOtherTabs_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var currentTab = GetCurrentTab(e.Parameter);
            if (currentTab != null)
            {
                currentTab.Owner.CloseOtherTabs(currentTab);
            }
        }

        private void CloseOffsideTabs_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var currentTab = GetCurrentTab(e.Parameter);
            if (currentTab != null)
            {
                currentTab.Owner.CloseOffsideTabs(currentTab);
            }
        }
        #endregion 绑定命令

        class AePageViewModel : INotifyPropertyChanged
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

            public string Title { get; }

            public IShop Shop { get; }

            public QBrowserBaseContent Content { get; }


            public AePageViewModel(IShop shop, string title, string uriString)
            {
                if (uriString is null)
                {
                    throw new ArgumentNullException(nameof(uriString));
                }

                Shop = shop ?? throw new ArgumentNullException(nameof(shop));

                if (string.IsNullOrEmpty(title))
                {
                    Title = shop.DisplayName;
                }
                else
                {
                    Title = string.Join(" - ", title, shop.DisplayName);
                }
                Content = new AeBrowserContent(shop.ShopId, uriString);
            }
        }
    }
}
