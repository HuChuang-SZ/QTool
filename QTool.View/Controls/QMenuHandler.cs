using CefSharp.Wpf;
using CefSharp;
using System;
using System.Collections.Generic;
using System.Windows;
using QTool.View.Models;
using QTool.View.Contents;

namespace QTool.View.Controls
{

    public class QMenuHandler : IContextMenuHandler
    {

        private readonly QBrowserBaseContent _content;

        public QMenuHandler(QBrowserBaseContent content)
        {
            _content = content;
        }

        class QMenuCommands
        {
            public const CefMenuCommand VideoView = (CefMenuCommand)1000;
            public const CefMenuCommand SaveAs = (CefMenuCommand)1001;
            public const CefMenuCommand ImageView = (CefMenuCommand)1002;
            public const CefMenuCommand ImageCopyUri = (CefMenuCommand)1003;
            public const CefMenuCommand ImageBatchDownload = (CefMenuCommand)1004;
            public const CefMenuCommand OpenLink = (CefMenuCommand)1005;
            public const CefMenuCommand CopyLink = (CefMenuCommand)1006;
        }

        void IContextMenuHandler.OnBeforeContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model)
        {
            int index = 0;
            if (parameters.TypeFlags.HasFlag(ContextMenuType.Link))
            {
                model.InsertItemAt(index++, QMenuCommands.OpenLink, "在新标签页中打开链接");
                model.InsertItemAt(index++, QMenuCommands.CopyLink, "复制链接地址");
                model.InsertSeparatorAt(index++);
            }

            switch (parameters.MediaType)
            {
                case ContextMenuMediaType.None:
                    break;
                case ContextMenuMediaType.Image:
                    model.InsertItemAt(index++, QMenuCommands.SaveAs, "将图片另存为");
                    model.InsertItemAt(index++, QMenuCommands.ImageView, "查看图片");
                    model.InsertItemAt(index++, QMenuCommands.ImageCopyUri, "复制图片链接");
                    model.InsertSeparatorAt(index++);
                    break;
                case ContextMenuMediaType.Video:
                    model.InsertItemAt(index++, QMenuCommands.VideoView, "查看视频(&P)");
                    model.InsertItemAt(index++, QMenuCommands.SaveAs, "将视频另存为");
                    model.InsertSeparatorAt(index++);
                    break;
                case ContextMenuMediaType.Audio:
                    break;
                case ContextMenuMediaType.Canvas:
                    break;
                case ContextMenuMediaType.File:
                    break;
                case ContextMenuMediaType.Plugin:
                    break;
                default:
                    break;
            }

            model.InsertItemAt(index++, CefMenuCommand.Reload, "刷新(&R)");
            model.InsertItemAt(index++, QMenuCommands.ImageBatchDownload, "下载全部图片");
            model.InsertSeparatorAt(index++);
        }

        bool IContextMenuHandler.OnContextMenuCommand(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, CefMenuCommand commandId, CefEventFlags eventFlags)
        {
            switch (commandId)
            {
                case CefMenuCommand.Reload:
                    browser.Reload();
                    return true;
                case QMenuCommands.VideoView:
                    VideoView.ShowDialog(parameters.SourceUrl, browserControl as ChromiumWebBrowser);
                    return true;
                case QMenuCommands.ImageCopyUri:
                    Clipboard.SetText(parameters.SourceUrl);
                    return true;
                case QMenuCommands.ImageView:
                    ImageView.ShowDialog(parameters.SourceUrl, browserControl as ChromiumWebBrowser);
                    return true;
                case QMenuCommands.SaveAs:
                    browserControl.StartDownload(parameters.SourceUrl);
                    return true;
                case QMenuCommands.ImageBatchDownload:
                    OnImageBatchDownload(browser, browserControl as ChromiumWebBrowser);
                    return true;
                case QMenuCommands.CopyLink:
                    Clipboard.SetText(parameters.LinkUrl);
                    return true;
                case QMenuCommands.OpenLink:
                    _content.AddTab(parameters.LinkUrl, WebBrowserTabOpenWith.Foreground);
                    return true;
                default:
                    break;
            }
            return false;
        }

        private void OnImageBatchDownload(IBrowser browser, ChromiumWebBrowser chromiumWebBrowser)
        {
            browser.MainFrame.GetSource(new QHtmlSourceVisitor(html =>
            {
                ImageDownloader.ShowDialog(html, chromiumWebBrowser);
            }));
        }

        void IContextMenuHandler.OnContextMenuDismissed(IWebBrowser browserControl, IBrowser browser, IFrame frame)
        {
            var webBrowser = (ChromiumWebBrowser)browserControl;
            Action setContextAction = delegate ()
            {
                webBrowser.ContextMenu = null;
            };
            webBrowser.Dispatcher.Invoke(setContextAction);
        }

        bool IContextMenuHandler.RunContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model, IRunContextMenuCallback callback)
        {
            //return false 才可以弹出;
            return false;
        }

        //下面这个官网Example的Fun,读取已有菜单项列表时候,实现的IEnumerable,如果不需要,完全可以注释掉;不属于IContextMenuHandler接口规定的
        private static IEnumerable<Tuple<string, CefMenuCommand, bool>> GetMenuItems(IMenuModel model)
        {
            for (var i = 0; i < model.Count; i++)
            {
                var header = model.GetLabelAt(i);
                var commandId = model.GetCommandIdAt(i);
                var isEnabled = model.IsEnabledAt(i);
                yield return new Tuple<string, CefMenuCommand, bool>(header, commandId, isEnabled);
            }
        }

        private static string _imageSaveFolder;
        public static string ImageSaveFolder
        {
            get
            {
                if (_imageSaveFolder == null)
                    _imageSaveFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                return _imageSaveFolder;
            }

            set
            {
                _imageSaveFolder = value;
            }
        }
    }
}
