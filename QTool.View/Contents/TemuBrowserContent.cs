using CefSharp;
using QTool.View.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTool.View.Contents
{
    public class TemuBrowserContent : QBrowserBaseContent
    {

        public TemuBrowserContent(int shopId, string uriString)
            : base(QModule.TemuBrowser, shopId, uriString)
        {
        }

        protected override QWebBrowserBase CreateContent(string address)
        {
            return new QWebBrowserByTemu(this, address);
        }

        private readonly static Dictionary<string, QBrowserBaseContent> _contentMaps = new Dictionary<string, QBrowserBaseContent>();

        public override string Icon => "/Resources/temuIcon.ico";

        public static QBrowserBaseContent Create(int shopId, string uriString)
        {
            string key = string.Join("-", shopId, uriString);
            lock (_contentMaps)
            {
                if (!_contentMaps.TryGetValue(key, out QBrowserBaseContent content))
                {
                    content = new TemuBrowserContent(shopId, uriString);
                    _contentMaps.Add(key, content);
                }
                return content;
            }
        }

        public static void RemoveShop(string userId)
        {
            lock (_contentMaps)
            {
                var allKeys = _contentMaps.Keys.ToArray();
                foreach (var key in allKeys)
                {
                    if (_contentMaps.TryGetValue(key, out QBrowserBaseContent content) && content.UserId == userId)
                    {
                        _contentMaps.Remove(key);
                    }
                }
            }
        }
    }
}
