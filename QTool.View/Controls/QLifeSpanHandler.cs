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

namespace QTool.View.Controls
{
    class QLifeSpanHandler : LifeSpanHandler
    {
        private readonly QBrowserBaseContent _browser;

        public QLifeSpanHandler(QBrowserBaseContent browser)
        {
            _browser = browser;
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
            if (_newTabs.Contains(targetDisposition))
            {
                WebBrowserTabOpenWith openWith;
                if (Keyboard.Modifiers != ModifierKeys.Control && targetDisposition != WindowOpenDisposition.NewBackgroundTab)
                {
                    openWith = WebBrowserTabOpenWith.Foreground;
                }
                else
                {
                    openWith = WebBrowserTabOpenWith.Background;
                }
                newBrowser = _browser.AddTab(targetUrl, openWith);
                return true;
            }
            else
            {
                return base.OnBeforePopup(chromiumWebBrowser, browser, frame, targetUrl, targetFrameName, targetDisposition, userGesture, popupFeatures, windowInfo, browserSettings, ref noJavascriptAccess, out newBrowser);
            }
        }
    }
}
