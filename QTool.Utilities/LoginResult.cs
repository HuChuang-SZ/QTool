using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTool
{
    public class LoginResult
    {
        public int AccountId { get; set; }

        public LoginShop[] Shops { get; set; }

        public DateTime Expire { get; set; }
    }
}
