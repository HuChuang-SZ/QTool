using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTool
{
    public class CookieChangedEventArgs : EventArgs
    {
        public ReadOnlyCollection<QCookie> AddCookies { get; }

        public CookieChangedEventArgs(IList<QCookie> addCookies = null)
        {
            if (addCookies == null)
            {
                addCookies = Array.Empty<QCookie>();
            }

            AddCookies = new ReadOnlyCollection<QCookie>(addCookies);
        }

    }
}
