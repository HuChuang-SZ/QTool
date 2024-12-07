using System;

namespace QTool.PDF
{
    [Flags]
    public enum PFontStyle : byte
    {
        Normal = 0,
        Bold = 1,
        Italic = 2,
        Underline = 4,
        Strikethru = 8,
    }
}
