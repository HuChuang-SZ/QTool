using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTool.Controls
{
    [Flags]
    public enum ShadeCloseMode : byte
    {
        NoClose = 0,
        OnlyCloseBtn = 1,
        OnlyBlankClick = 2,
        Both = 3,
    }
}
