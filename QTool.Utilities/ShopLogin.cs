using System;

namespace QTool
{
    public class ShopLogin : IShopLogin
    {
        public ShopLogin(IShop shop, QCookieManager cookieManager)
        {
            Shop = shop ?? throw new ArgumentNullException(nameof(shop));
            CookieManager = cookieManager ?? throw new ArgumentNullException(nameof(cookieManager));
        }

        public IShop Shop { get; }

        public QCookieManager CookieManager { get; }
    }

    public interface IShopLogin
    {
        IShop Shop { get; }

        QCookieManager CookieManager { get; }
    }


    public class NewShopLogin : IShopLogin
    {
        public NewShopLogin(QCookieManager cookieManager)
        {
            CookieManager = cookieManager ?? throw new ArgumentNullException(nameof(cookieManager));
        }

        public QCookieManager CookieManager
        {
            get;
        }

        public IShop Shop
        {
            get
            {
                return null;
            }
        }
    }
}
