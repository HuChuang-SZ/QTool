
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace QTool.View.Controls
{
    public static class CefCookieExt
    {

        public static IList<QCookie> Convert(this IEnumerable<CefSharp.Cookie> cookies)
        {
            return cookies.Convert(Convert);
        }

        private static QCookie Convert(this CefSharp.Cookie cookie)
        {
            return new QCookie
            {
                Domain = cookie.Domain,
                Name = cookie.Name,
                Value = cookie.Value,
                Path = cookie.Path,
                Expires = cookie.Expires,
                Secure = cookie.Secure,
                HttpOnly = cookie.HttpOnly
            };
        }

        public static CefSharp.Cookie ConvertToCef(this QCookie cookie)
        {
            DateTime? expires;
            if (cookie.Expires == DateTime.MinValue)
            {
                expires = null;
            }
            else
            {
                expires = cookie.Expires;
            }
            return new CefSharp.Cookie()
            {
                Name = cookie.Name,
                Value = cookie.Value,
                Domain = cookie.Domain,
                Path = cookie.Path,
                Secure = cookie.Secure,
                HttpOnly = cookie.HttpOnly,
                Expires = expires,
            };
        }
    }
}
