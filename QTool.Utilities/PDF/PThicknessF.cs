using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTool.PDF
{
    public struct PThicknessF
    {
        public PThicknessF(float uniformLength)
            : this(uniformLength, uniformLength)
        {

        }

        public PThicknessF(float leftAndRight, float topAndBottom)
            : this(leftAndRight, topAndBottom, leftAndRight, topAndBottom)
        {
        }

        public PThicknessF(float left, float top, float right, float bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public float Top { get; }

        public float Left { get; }

        public float Right { get; }

        public float Bottom { get; }
    }
}
