using QTool.Controls;
using QTool.View.Contents;
using QTool.View.DAL;
using QTool.View.Models;
using QTool.View.ViewModels;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace QTool.View
{
    /// <summary>
    /// ShopManager.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : QWindow
    {
        public MainWindow()
        {
            var result = new LoginResult()
            {
                AccountId = 1,
                Shops = new LoginShop[]
                              {
                                new LoginShop(){ ShopId = 1, Platform = QPlatform.AliExpress, ShopIdentity = "cn1541248744bjyk", DisplayName = "店铺1", },
                                new LoginShop(){ ShopId = 2, Platform = QPlatform.AliExpress, ShopIdentity = "cn1019492009gheae", DisplayName = "店铺2", }
                              }
            };
            QContext.Current.LoginSuccess(result);

            DataContext = MainWindowViewModel.Current;

            InitializeComponent();

            Loaded += MainWindow_Loaded;

            Initialize();

            HelpTip.AddSteps(this, new HelpTipStep[] {
                    new HelpTipStep("使用引导","欢迎使用QTool，我们已为你准备了快速引导，只需 1 分钟即可了解新版特性，立即开始吧！"),
                    new HelpTipStep("标签管理","标签管理只要用于标签快速切换，不同店铺不同功能拥有独立选项卡。","eTab"),
                    new HelpTipStep("店铺列表","店铺列表主要用于店铺切换、未读留言数、登录状态等功能。","eShop"),
            }, 1);
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            QContext.Current.StartUnread();
        }

        public override bool IsMain => true;

        private void Initialize()
        {
            MainWindowViewModel.Current.IsWaiting = true;
            AsyncHelper.Exec(() =>
            {
                return AccountDAL.Current.GetParameters(QContext.Current.AccountId);
            }, result =>
            {
                if (!result.HasError)
                {
                    var param = result.Result;
                    if (param != null)
                    {
                        MainWindowViewModel.Current.CurrentModule = QModuleInfo.FindByModuleId(param.ModuleId);
                        var shop = MainWindowViewModel.Current.Shops.FirstOrDefault(s => s.ShopId == param.ShopId);
                        if (shop != null)
                        {
                            MainWindowViewModel.Current.CurrentShop = shop;
                        }
                        if (param.IsMaximize)
                        {
                            WindowState = WindowState.Maximized;
                        }
                        else
                        {
                            if (param.WindowTop + param.WindowHeight < SystemParameters.PrimaryScreenHeight && param.WindowLeft + param.WindowWidth < SystemParameters.PrimaryScreenWidth)
                            {
                                WindowStartupLocation = WindowStartupLocation.Manual;
                                Width = param.WindowWidth;
                                Height = param.WindowHeight;
                                Top = param.WindowTop;
                                Left = param.WindowLeft;
                            }
                        }
                    }
                }
                MainWindowViewModel.Current.IsWaiting = false;
                MainWindowViewModel.Current.Initialized();
            });
        }


        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            var param = new AccountParameters
            {
                Id = QContext.Current.AccountId,
                ModuleId = MainWindowViewModel.Current.CurrentModule.Id,
                ShopId = MainWindowViewModel.Current.CurrentShop?.ShopId,
                IsMaximize = WindowState == WindowState.Maximized
            };

            if (!param.IsMaximize)
            {
                param.WindowTop = Top;
                param.WindowLeft = Left;
                param.WindowHeight = Height;
                param.WindowWidth = Width;
            }

            AccountDAL.Current.SetParameters(param);
            Application.Current.Shutdown();
        }

        #region 绑定命令
        private void Tab_CanExceute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = GetCurrentTab(e.Parameter) != null;
        }

        private QWebBrowserBase GetCurrentTab(object parameter)
        {
            if (parameter is QWebBrowserBase tab)
            {
                return tab;
            }
            else
            {
                return (MainWindowViewModel.Current.CurrentContent as QBrowserBaseContent)?.Content;
            }
        }

        private void RefreshTab_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            GetCurrentTab(e.Parameter)?.Refresh();
        }

        private void CopyTab_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var currentTab = GetCurrentTab(e.Parameter);
            if (currentTab != null)
            {
                currentTab.Owner.AddTab(currentTab.Address, WebBrowserTabOpenWith.Foreground);
            }
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
    }
}
