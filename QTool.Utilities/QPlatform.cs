using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTool
{
    public enum QPlatform : byte
    {
        [Display("速卖通")]
        AliExpress = 0,

        [Display("Temu")]
        Temu = 1,
    }
}
