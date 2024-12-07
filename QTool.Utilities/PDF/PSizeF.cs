using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTool.PDF
{
    public struct PSizeF
    {
        public PSizeF(float width, float height, PUnit unit = PUnit.Pixel)
        {
            switch (unit)
            {
                case PUnit.Pixel:
                    Width = width;
                    Height = height;
                    break;
                case PUnit.Centimeter:
                    Width = width.CmToPixel();
                    Height = height.CmToPixel();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(unit));
            }
        }

        public float Height { get; }

        public float Width { get; }

        public static PSizeF Create(int widthCm, int heightCm)
        {
            return new PSizeF(GlobalHelper.CmToPixel(widthCm), GlobalHelper.CmToPixel(heightCm));
        }
    }

    public enum PUnit
    {
        /// <summary>
        /// 像素
        /// </summary>
        Pixel,

        /// <summary>
        /// 厘米
        /// </summary>
        Centimeter,
    }
}
