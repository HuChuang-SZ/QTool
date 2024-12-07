using CefSharp;
using CefSharp.Wpf;
using QTool.Controls;
using QTool.View.Utilities;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;

namespace QTool.View
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            if (ProcessHelper.CheckProcess())
            {
                Shutdown();
            }
            else
            {
                base.OnStartup(e);
                QMessageHelper.Init(QMessageBox.Show);
                RegisterEvents();

                var result = new LoginResult()
                {
                    AccountId = 1,
                    Shops = new LoginShop[] {
                        new LoginShop(){ ShopId = 1, Platform = QPlatform.AliExpress, ShopIdentity = "cn1541248712bbac", DisplayName = "店铺A", },
                        new LoginShop(){ ShopId = 1, Platform = QPlatform.AliExpress, ShopIdentity = "cn1541248712bba2", DisplayName = "店铺B", },
                    }
                };

                WinHelper.InitCef();
                QContext.Current.LoginSuccess(result);
                var dialog = new MainWindow();
                dialog.ShowDialog();
            }
        }


        private void RegisterEvents()
        {
            ////Task线程内未捕获异常处理事件
            //TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException; //Task异常 

            //UI线程未捕获异常处理事件（UI主线程）
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;

            //非UI线程未捕获异常处理事件(例如自己创建的一个子线程)
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            try
            {
                var exception = e.Exception as Exception;
                if (exception != null)
                {
                    HandleException(exception);
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
            finally
            {
                e.SetObserved();
            }
        }

        //非UI线程未捕获异常处理事件(例如自己创建的一个子线程)      
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                var exception = e.ExceptionObject as Exception;
                if (exception != null)
                {
                    HandleException(exception);
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
            finally
            {
                //ignore
            }
        }

        //UI线程未捕获异常处理事件（UI主线程）
        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            try
            {
                HandleException(e.Exception);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
            finally
            {
                e.Handled = true;
            }
        }
        private void HandleException(Exception ex)
        {
            QMessageWindow.Show(ex.ToString());
            LogHelper.WriteLog(ex);
            Shutdown();
        }
    }
}
