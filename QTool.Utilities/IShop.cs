using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTool
{
    public interface IShop
    {

        int ShopId { get; }

        QPlatform Platform { get; }

        string ShopIdentity { get; }

        string DisplayName { get; }
    }
}
