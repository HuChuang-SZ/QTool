using CefSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTool.View.Controls
{
    public class QCookieVisitor : ICookieVisitor
    {
        public void Dispose()
        {

        }

        //public bool IsSuccess = false;
        public bool Visit(Cookie cookie, int count, int total, ref bool deleteCookie)
        {
            deleteCookie = false;
            _cookies.Add(cookie);
            return true;

        }

        public void ClearAll()
        {
            _cookies.Clear();
        }

        private List<Cookie> _cookies = new List<Cookie>();

        public ReadOnlyCollection<Cookie> Cookies
        {
            get
            {
                return new ReadOnlyCollection<Cookie>(_cookies);
            }
        }
    }
}
