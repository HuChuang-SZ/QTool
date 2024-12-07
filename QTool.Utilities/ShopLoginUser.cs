using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTool
{
    public class ShopLoginUser
    {
        [BsonId]
        public string UserId { get; set; }

        public QPlatform Platform { get; set; }


        public string Account { get; set; }

        public string Password { get; set; }
    }
}
