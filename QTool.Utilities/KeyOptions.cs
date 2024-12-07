using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTool
{
    [Flags]
    public enum KeyOptions : byte
    {
        None = 0,
        IgnoreCase = 1,
        IgnoreSpace = 2,
    }
}
