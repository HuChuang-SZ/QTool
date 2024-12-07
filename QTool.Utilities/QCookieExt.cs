using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace QTool
{
    public static class QCookieExt
    {
        public static IList<QCookie> Convert<TCookie>(this IEnumerable<TCookie> cookies, Func<TCookie, QCookie> convert)
        {
            if (cookies != null)
            {
                List<QCookie> list = new List<QCookie>(cookies.Count());
                foreach (var item in cookies)
                {
                    list.Add(convert(item));
                }
                return list;
            }
            else
                return null;
        }

        public static IList<QCookie> Convert(this IEnumerable<Cookie> cookies)
        {
            return Convert(cookies, Convert);
        }

        public static IList<QCookie> Convert(this CookieCollection cookies)
        {
            return Convert(cookies.Cast<Cookie>(), Convert);
        }

        private static QCookie Convert(this Cookie cookie)
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
    }
}
