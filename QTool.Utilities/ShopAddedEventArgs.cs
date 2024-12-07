using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTool
{
    public class ShopAddedEventArgs : EventArgs
    {
        public ShopAddedEventArgs(QCookieManager LoginCookie, ShopLoginUser loginUser, ShopBindModel[] loginShops)
        {
            if (LoginCookie is null)
            {
                throw new ArgumentNullException(nameof(LoginCookie));
            }
            if (loginShops is null || loginShops.Length == 0)
            {
                throw new ArgumentNullException(nameof(loginShops));
            }

            this.LoginCookie = LoginCookie;
            LoginUser = loginUser;
            LoginShops = loginShops;
        }

        public QCookieManager LoginCookie { get; }

        public ShopLoginUser LoginUser { get; }

        public ShopBindModel[] LoginShops { get; }
    }
}
