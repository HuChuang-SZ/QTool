using System;

namespace QTool.PDF
{
    [Flags]
    public enum PBorder : byte
    {
        None = 0,
        Top = 1,
        Bottom = 2,
        Left = 4,
        Right = 8,


        TopBottom = 3,
        LeftTop = 5,
        LeftBottom = 6,
        LeftTopBottom = 7,
        TopRight = 9,
        RightBottom = 10,
        TopRightBottom = 11,
        LeftRight = 12,
        LeftTopRight = 13,
        LeftRightBottom = 14,
        All = 15,
    }
}
