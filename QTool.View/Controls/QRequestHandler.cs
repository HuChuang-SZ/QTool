using CefSharp;
using CefSharp.Handler;
using QTool.View.Contents;
using QTool.View.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace QTool.View.Controls
{
    public class QRequestHandler : RequestHandler
    {
        private readonly QBrowserBaseContent _content;

        public QRequestHandler(QBrowserBaseContent content)
        {
            _content = content;
        }



        protected override bool OnBeforeBrowse(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, bool userGesture, bool isRedirect)
        {
            if (request.TransitionType == TransitionType.LinkClicked)
            {
                if (Keyboard.Modifiers == ModifierKeys.Control)
                {
                    _content.AddTab(request.Url, WebBrowserTabOpenWith.Background);
                    return true;
                }
                else if (request.TransitionType == TransitionType.LinkClicked && Keyboard.Modifiers == (ModifierKeys.Shift | ModifierKeys.Control))
                {
                    _content.AddTab(request.Url, WebBrowserTabOpenWith.Foreground);
                    return true;
                }
            }
            return base.OnBeforeBrowse(chromiumWebBrowser, browser, frame, request, userGesture, isRedirect);
        }
        protected override IResourceRequestHandler GetResourceRequestHandler(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, bool isNavigation, bool isDownload, string requestInitiator, ref bool disableDefaultHandling)
        {
            return new QResourceRequestHandler(_content);
        }
    }
    public class QResourceRequestHandler : ResourceRequestHandler
    {

        private readonly QBrowserBaseContent _content;

        public QResourceRequestHandler(QBrowserBaseContent content)
        {
            _content = content;
        }


        protected override CefReturnValue OnBeforeResourceLoad(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, IRequestCallback callback)
        {
            var headers = request.Headers;
            headers["Accept-Language"] = QRequestHeader.AcceptLanguage;
            headers["User-Agent"] = QRequestHeader.UserAgent;
            headers["sec-ch-ua"] = QRequestHeader.sec_ch_ua;
            _content.SetRequestHeaders(headers);
            request.Headers = headers;
            return CefReturnValue.Continue;
        }
    }
}
