using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTool
{
    public class ShopLoginCookie : IExtDatas
    {
        [BsonId]
        public string UserId { get; set; }

        public QPlatform Platform { get; set; }

        public QCookie[] Cookies { get; set; }

        public Dictionary<byte, string> ExtDatas { get; set; }

        public QCookieManager GetCookieManager()
        {
            QCookieManager cookieManager;
            switch (Platform)
            {
                case QPlatform.AliExpress:
                    cookieManager = new QCookieManagerByAe(UserId);
                    break;
                case QPlatform.Temu:
                    cookieManager = new QCookieManagerByTemu(UserId);
                    break;
                default:
                    throw new QException($"无法创建“{Platform}”平台CookieManager");
            }
            cookieManager.Cookies = Cookies;
            cookieManager.ExtDatas = ExtDatas;
            return cookieManager;
        }
    }
}
