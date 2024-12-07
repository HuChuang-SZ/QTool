using CefSharp;
using CefSharp.Wpf;
using QTool.View.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace QTool.View.Controls
{
    public static class ChromiumWebBrowserExt
    {
        public static QWebBrowserBase GetOwner(ChromiumWebBrowser obj)
        {
            return (QWebBrowserBase)obj.GetValue(OwnerProperty);
        }

        public static void SetOwner(ChromiumWebBrowser obj, QWebBrowserBase value)
        {
            obj.SetValue(OwnerProperty, value);
        }

        // Using a DependencyProperty as the backing store for ParentBrowser.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OwnerProperty =
            DependencyProperty.RegisterAttached("Owner", typeof(QWebBrowserBase), typeof(ChromiumWebBrowser), new PropertyMetadata(null));


        public static void AddTab(this ChromiumWebBrowser webBrowser, string address, WebBrowserTabOpenWith openWith)
        {
            if (webBrowser.CheckAccess())
            {
                var browserBase = GetOwner(webBrowser);

                if (browserBase != null && browserBase.Owner != null)
                {
                    browserBase.Owner.AddTab(address, openWith);
                }
            }
            else
            {
                webBrowser.Dispatcher.Invoke(new Action(() =>
                {
                    AddTab(webBrowser, address, openWith);
                }));
            }
        }
    }
}
