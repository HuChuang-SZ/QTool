using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Concurrent;
using System.Text;

namespace QTool
{
    public abstract class QCookieManager : IExtDatas
    {
        private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, QCookie>> _domainMaps = new ConcurrentDictionary<string, ConcurrentDictionary<string, QCookie>>();
        public QCookieManager(QPlatform platform, Uri defaultUri, string userId)
        {
            UserId = userId;
            DefaultUri = defaultUri ?? throw new ArgumentNullException(nameof(defaultUri));
            Platform = platform;
        }

        public string UserId { get; }

        public Uri DefaultUri { get; }

        public QPlatform Platform { get; }


        private void AddCookie(QCookie cookie)
        {
            var domainKey = GetDomainKey(cookie);
        retry:
            if (!_domainMaps.TryGetValue(domainKey, out ConcurrentDictionary<string, QCookie> nameMaps))
            {
                nameMaps = new ConcurrentDictionary<string, QCookie>();

                if (!_domainMaps.TryAdd(domainKey, nameMaps))
                {
                    goto retry;
                }
            }

            nameMaps.AddOrUpdate(GetNameKey(cookie), cookie, (key, oldValue) => cookie);
        }

        public void AddCookies(IList<QCookie> cookies)
        {
            if (cookies != null)
            {
                foreach (var cookie in cookies)
                {
                    AddCookie(cookie);
                }
            }

            OnCookieChanged(new CookieChangedEventArgs(cookies));
        }

        private EventHandler<CookieChangedEventArgs> _cookiesChanged;
        public event EventHandler<CookieChangedEventArgs> CookiesChanged
        {
            add
            {
                _cookiesChanged = value;
            }

            remove
            {
                _cookiesChanged = value;
            }
        }

        protected virtual void OnCookieChanged(CookieChangedEventArgs e)
        {
            _cookiesChanged?.Invoke(this, e);
        }

        public int Count
        {
            get
            {
                return _domainMaps.Values.Sum(val => val.Count);
            }
        }

        /// <summary>
        /// 获取Cookie
        /// </summary>
        /// <returns></returns>
        public QCookie[] Cookies
        {
            get
            {
                return GetCookies(DefaultUri).ToArray();
            }
            set
            {
                if (value != null)
                {
                    AddCookies(value);
                }
            }
        }

        public Dictionary<byte, string> ExtDatas { get; set; }

        protected virtual IEnumerable<QCookie> GetCookies(Uri uri)
        {
            foreach (var domainKey in GetDomainKeys(uri))
            {
                if (_domainMaps.TryGetValue(domainKey, out ConcurrentDictionary<string, QCookie> nameMaps))
                {
                    foreach (var cookie in nameMaps.Values.ToArray())
                    {
                        if (cookie.Expired())
                        {
                            var nameKey = GetNameKey(cookie);
                            if (nameMaps.TryRemove(nameKey, out QCookie dCookie))
                            {
                                if (dCookie != cookie && !dCookie.Expired())
                                {
                                    nameMaps.AddOrUpdate(nameKey, dCookie, (key, oldVal) => dCookie);
                                }
                            }
                        }
                        else if (uri.LocalPath.IndexOf(cookie.Path, 0, StringComparison.CurrentCultureIgnoreCase) == 0)
                        {
                            yield return cookie;
                        }
                    }
                }
            }
        }


        public const char _periodChar = '.';
        /// <summary>
        /// 返回Uri全部子域名
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetDomainKeys(Uri uri)
        {
            var host = uri.Host.ToLower();

            yield return host;

            var lastIndex = host.LastIndexOf('.');
            if (lastIndex != -1)
            {
                for (int i = 0; i < lastIndex; i++)
                {
                    if (host[i] == _periodChar)
                    {
                        yield return host.Substring(i + 1);
                    }
                }
            }
        }


        private const string _semicolonStr = "; ", _equalStr = "=";

        public string GetCookiesHeader(Uri uri)
        {
            var cookies = new StringBuilder();

            bool isFirst = true;
            foreach (var cookie in GetCookies(uri))
            {
                if (isFirst)
                    isFirst = false;
                else
                    cookies.Append(_semicolonStr);

                cookies.Append(cookie.Name).Append(_equalStr).Append(cookie.Value);
            }

            return cookies.ToString();
        }


        private static string GetDomainKey(QCookie cookie)
        {
            if (cookie.Domain == null)
                return string.Empty;
            else if (cookie.Domain[0] == _periodChar)
                return cookie.Domain.Substring(1).ToLower();
            else
                return cookie.Domain.ToLower();
        }


        private string GetNameKey(QCookie cookie)
        {
            return string.Concat(cookie.Name, cookie.Path).ToLower();
        }



        public void ClearAll()
        {
            _domainMaps.Clear();
            OnCookieChanged(new CookieChangedEventArgs());
        }



    }
}
