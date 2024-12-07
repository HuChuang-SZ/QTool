using System;

namespace QTool
{
    /// <summary>
    /// AE店铺配置
    /// </summary>
    public class QCookieManagerByTemu : QCookieManager
    {
        public const string DefaultUriString = "https://seller.kuajingmaihuo.com/";

        public QCookieManagerByTemu(string userId = null)
            : base(QPlatform.Temu, new Uri(DefaultUriString), userId)
        {

        }
    }
}
