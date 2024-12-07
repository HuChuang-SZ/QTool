using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTool
{
    public class ShopAssociated
    {
        [BsonId]
        public int ShopId { get; set; }

        public string UserId { get; set; }
    }
}
