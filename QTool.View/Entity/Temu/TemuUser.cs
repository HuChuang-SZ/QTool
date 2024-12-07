using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTool.View.Entity.Temu
{
    public class TemuUser
    {
        public string UserId { get; set; }

        public ShopBindModel[] Shops { get; set; }
    }
}
