using System.Linq;
using QTool.View.Models;
using System.Collections.Generic;
using QTool.View.DAL;

namespace QTool.View.Contents
{
    public class AeBrowserContent : QBrowserBaseContent
    {
        public AeBrowserContent(int shopId, string uriString)
            : base(QModule.AeBrowser, shopId, uriString)
        {
        }
        public override string Icon => "/Resources/aeIcon.ico";

        protected override QWebBrowserBase CreateContent(string address)
        {
            return new QWebBrowserByAe(this, address);
        }

        private const string ChannelIdKey = "{channelId}";
        protected override string GetDefaultUri()
        {
            if (UriString.IndexOf(ChannelIdKey) != -1)
            {
                if (QContext.Current.TryGetCookieManager(UserId, out QCookieManager cookieManager))
                {
                    var channelId = cookieManager.GetExtValue(ExtDatasKeys.ChannelId);
                    if (!string.IsNullOrEmpty(channelId))
                    {
                        return UriString.Replace(ChannelIdKey, channelId);
                    }
                }

                return QCookieManagerByAe.DefaultUriString;
            }
            else
            {
                return UriString;
            }
        }



        private readonly static Dictionary<string, QBrowserBaseContent> _contentMaps = new Dictionary<string, QBrowserBaseContent>();
        public static QBrowserBaseContent Create(int shopId, string uriString)
        {
            string key = string.Join("-", shopId, uriString);
            lock (_contentMaps)
            {
                if (!_contentMaps.TryGetValue(key, out QBrowserBaseContent content))
                {
                    content = new AeBrowserContent(shopId, uriString);
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
