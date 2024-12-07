using CefSharp;
using CefSharp.Wpf;
using QTool.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace QTool.View.Controls
{
    internal class QDownloadHandler : IDownloadHandler
    {
        public bool CanDownload(IWebBrowser chromiumWebBrowser, IBrowser browser, string url, string requestMethod)
        {
            return true;
        }

        public void OnBeforeDownload(IWebBrowser chromiumWebBrowser, IBrowser browser, DownloadItem downloadItem, IBeforeDownloadCallback callback)
        {
            if (!callback.IsDisposed)
            {
                var file = GetDownloadFile(downloadItem.SuggestedFileName);
                using (callback)
                {
                    callback.Continue(file, showDialog: true);
                }
            }
        }

        private static string GetDownloadFile(string fileName, int index = 0)
        {
            string saveFile;
            if (index == 0)
            {
                saveFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", fileName);
            }
            else
            {
                saveFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", Path.GetFileNameWithoutExtension(fileName) + index.ToString("(0)") + Path.GetExtension(fileName));
            }
            if (File.Exists(saveFile))
            {
                return GetDownloadFile(saveFile, index + 1);
            }
            else
            {
                return saveFile;
            }
        }

        public void OnDownloadUpdated(IWebBrowser chromiumWebBrowser, IBrowser browser, DownloadItem downloadItem, IDownloadItemCallback callback)
        {
            //todo 文件下载成功后关闭选项卡
            if (downloadItem.IsComplete)
            {
                if (downloadItem != null)
                {
                    var webBrowser = chromiumWebBrowser as ChromiumWebBrowser;
                    if (webBrowser != null)
                    {
                        ShowDownloadFileAndCloseTab(downloadItem, webBrowser);
                    }
                }
            }
        }

        private static void ShowDownloadFileAndCloseTab(DownloadItem downloadItem, ChromiumWebBrowser webBrowser)
        {
            if (webBrowser.CheckAccess())
            {
                QMessageBox.Show($"文件下载成功，文件已保存到“{downloadItem.FullPath}”路径。");

                if (downloadItem.OriginalUrl == webBrowser.Address)
                {
                    ChromiumWebBrowserExt.GetOwner(webBrowser)?.Close();
                }
            }
            else
            {
                webBrowser.Dispatcher.Invoke(() =>
                {
                    ShowDownloadFileAndCloseTab(downloadItem, webBrowser);
                });
            }
        }
    }
}
