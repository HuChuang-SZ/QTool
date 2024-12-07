using CefSharp.Wpf;
using CefSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using CefSharp.Handler;
using QTool.View.Contents;
using QTool.View.Models;
using System.Windows;

namespace QTool.View.Controls
{
    class QLifeSpanHandlerByShop : LifeSpanHandler
    {
        private readonly IShop _shop;
        private readonly Window _owner;

        public QLifeSpanHandlerByShop(Window owner, IShop shop)
        {
            _owner = owner;
            _shop = shop;
        }

        private readonly WindowOpenDisposition[] _newTabs = new WindowOpenDisposition[]
        {
                WindowOpenDisposition.SwitchToTab,
                WindowOpenDisposition.SingletonTab,
                WindowOpenDisposition.NewForegroundTab,
                WindowOpenDisposition.NewBackgroundTab,
                WindowOpenDisposition.NewPopup,
                WindowOpenDisposition.NewWindow,
        };

        protected override bool OnBeforePopup(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, string targetUrl, string targetFrameName, WindowOpenDisposition targetDisposition, bool userGesture, IPopupFeatures popupFeatures, IWindowInfo windowInfo, IBrowserSettings browserSettings, ref bool noJavascriptAccess, out IWebBrowser newBrowser)
        {
            newBrowser = OpenWindow(targetUrl);
            return true;
            //if (_newTabs.Contains(targetDisposition))
            //{
            //    WebBrowserTabOpenWith openWith;
            //    if (Keyboard.Modifiers != ModifierKeys.Control && targetDisposition != WindowOpenDisposition.NewBackgroundTab)
            //    {
            //        openWith = WebBrowserTabOpenWith.Foreground;
            //    }
            //    else
            //    {
            //        openWith = WebBrowserTabOpenWith.Background;
            //    }
            //    newBrowser = _browser.AddTab(targetUrl, openWith);
            //    return true;
            //}
            //else
            //{
            //    return base.OnBeforePopup(chromiumWebBrowser, browser, frame, targetUrl, targetFrameName, targetDisposition, userGesture, popupFeatures, windowInfo, browserSettings, ref noJavascriptAccess, out newBrowser);
            //}
        }

        private IWebBrowser OpenWindow(string targetUrl)
        {

            if (_owner.CheckAccess())
            {
                AePage dialog = new AePage(_shop, null, targetUrl);
                dialog.Owner = _owner;
                var workArea = SystemParameters.WorkArea;
                dialog.Width = workArea.Width * 0.8;
                dialog.Height = workArea.Height * 0.9;
                dialog.Show();
                return dialog.GetBrowser();
            }
            else
            {
                return (IWebBrowser)_owner.Dispatcher.Invoke(new Func<string, IWebBrowser>(OpenWindow), targetUrl);
            }
        }
    }
}
