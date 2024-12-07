using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTool
{
    public class NotLoggedException : Exception
    {
        public NotLoggedException()
            : base("店铺未登录")
        {

        }

        public NotLoggedException(string shopName)
            : base($"店铺({shopName})未登录")
        {

        }
    }
}
