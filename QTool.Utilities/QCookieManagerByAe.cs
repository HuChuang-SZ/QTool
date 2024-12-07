using System;
using System.Collections.Generic;

namespace QTool
{
    /// <summary>
    /// AE店铺配置
    /// </summary>
    public class QCookieManagerByAe : QCookieManager
    {
        public const string DefaultUriString = "https://csp.aliexpress.com/";
        public QCookieManagerByAe(string userId = null)
            : base(QPlatform.AliExpress, new Uri(DefaultUriString), userId)
        {

        }

        protected override void OnCookieChanged(QTool.CookieChangedEventArgs e)
        {
            UpdateToken(e.AddCookies);
            base.OnCookieChanged(e);
        }

        private void UpdateToken(IList<QCookie> cookieList)
        {
            if (cookieList != null)
            {
                foreach (var cookie in cookieList)
                {
                    if (cookie?.Name == "_m_h5_tk")
                    {
                        var datas = cookie.Value?.Split('_');
                        if (datas?.Length == 2)
                        {
                            this.SetExtValue(ExtDatasKeys.Token, datas[0]);
                        }
                    }
                }
            }
        }


        protected override IEnumerable<QCookie> GetCookies(Uri uri)
        {
            foreach (var cookie in base.GetCookies(uri))
            {
                if (cookie.Name != "_lang")
                {
                    yield return cookie;
                }
            }

            yield return new QCookie()
            {
                Name = "_lang",
                Value = "zh_CN",
                Domain = ".aliexpress.com",
                Path = "/",
                Secure = true,
                Expires = DateTime.Now.AddYears(1),
                HttpOnly = false,
            };
        }

    }
}
