using CefSharp;
using QTool.Controls;

using QTool.View.Contents;
using QTool.View.Utilities;
using QTool.View.ViewModels;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace QTool.View.Models
{
    static class QWebBrowserDispatch
    {
        private readonly static double _keepMinutes = 5;
        private readonly static int _activeBrowserNum;
        private readonly static ConcurrentQueue<QWebBrowserBase> _markQueue = new ConcurrentQueue<QWebBrowserBase>();
        private static List<QWebBrowserBase> _items = new List<QWebBrowserBase>();
        static QWebBrowserDispatch()
        {
            Application.Current.Exit += Current_Exit;
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += Worker_DoWork;
            worker.RunWorkerAsync();

            var memory = new WinHelper().GetPhysicalMemory() / 1024 / 1024 / 1024.0;
            if (memory < 7)
            {
                _activeBrowserNum = 15;
            }
            else
            {
                _activeBrowserNum = 30;
            }

#if DEBUG
            _activeBrowserNum = 1;
            _keepMinutes = 0.1;
#endif

        }

        private static void Current_Exit(object sender, ExitEventArgs e)
        {
            _isRunning = false;
        }

        private static bool _isRunning = true;

        private static void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (_isRunning)
            {
                try
                {
                    while (_markQueue.TryDequeue(out QWebBrowserBase browser))
                    {
                        if (_items.Contains(browser))
                        {
                            _items.Remove(browser);
                        }
                        _items.Add(browser);
                    }

                    if (_items.Count > _activeBrowserNum)
                    {
                        DisposeBrowsers(_items.Take(_items.Count - _activeBrowserNum).Where(item => item.LastAccessMinutes() > _keepMinutes).ToArray());
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.WriteLog(ex);
                }
                finally
                {
                    Task.Delay(5000).Wait();
                }
            }
        }

        private static void DisposeBrowsers(QWebBrowserBase[] items)
        {
            if (items?.Length > 0)
            {
                var current = GetCurrentBrowser();
                foreach (var item in items)
                {
                    if (item != current)
                    {
                        _items.Remove(item);
                        item.DisposeWebBrowser();
                    }
                }
            }
        }

        private static MainWindow GetMainWindow()
        {
            if (Application.Current.CheckAccess())
            {
                foreach (var win in Application.Current.Windows)
                {
                    if (win is MainWindow)
                    {
                        return win as MainWindow;
                    }
                }
                return null;
            }
            else
            {
                return Application.Current.Dispatcher.Invoke(GetMainWindow);
            }
        }

        private static QWebBrowserBase GetCurrentBrowser()
        {
            var content = MainWindowViewModel.Current.CurrentContent as QBrowserBaseContent;
            return content?.Content;
        }

        public static void Mark(QWebBrowserBase browser)
        {
            _markQueue.Enqueue(browser);
        }
    }
}
