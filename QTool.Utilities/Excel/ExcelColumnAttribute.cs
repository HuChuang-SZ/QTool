using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace QTool
{
    public class ExcelColumnAttribute : Attribute
    {
        public ExcelColumnAttribute(string title, ExcelColumnType type)
        {
            Title = title;
            switch (type)
            {
                case ExcelColumnType.String:
                    Width = 200;
                    Alignment = HorizontalAlignment.Left;
                    break;
                case ExcelColumnType.Numeric:
                    Width = 120;
                    Alignment = HorizontalAlignment.Right;
                    break;
                case ExcelColumnType.Int64:
                case ExcelColumnType.Int32:
                    Format = "###";
                    Width = 120;
                    Alignment = HorizontalAlignment.Right;
                    break;
                case ExcelColumnType.DataTime:
                    Format = "yyyy-MM-dd HH:mm:ss";
                    Width = 160;
                    Alignment = HorizontalAlignment.Center;
                    break;
                case ExcelColumnType.Image:
                    Alignment = HorizontalAlignment.Center;
                    Height = 66;
                    Width = 66;
                    break;
                default:
                    break;
            }
            VerticalAlignment = VerticalAlignment.Center;
            Type = type;
        }

        public ExcelColumnType Type { get; private set; }

        public string Title { get; set; }

        public string Tooltip { get; set; }

        public short Width { get; set; }

        public short Height { get; set; }

        public string Format { get; set; }

        public HorizontalAlignment Alignment { get; set; }

        public VerticalAlignment VerticalAlignment { get; set; }

        public bool WrapText { get; set; }

        public bool IsBold { get; set; }

        public XSSFColor FontColor { get; set; }

        public int? FontSize { get; set; }

        public IFont CreateFont(IWorkbook workbook)
        {
            if (IsBold || FontColor != null || FontSize.HasValue)
            {
                var font = (XSSFFont)workbook.CreateFont();
                font.IsBold = IsBold;
                if (FontColor != null)
                    font.SetColor(FontColor);
                font.FontHeightInPoints = FontSize.Value;
                return font;
            }
            else
            {
                return null;
            }
        }
    }
}
