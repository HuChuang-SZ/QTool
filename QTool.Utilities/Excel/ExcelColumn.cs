using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;

namespace QTool
{
    /// <summary>
    /// Execl字段
    /// </summary>
    public abstract class ExcelColumn<TData>
    {
        public ExcelColumn(string title, ExcelColumnType type)
        {
            Title = title;
            Type = type;
            VAlign = VerticalAlignment.Center;
        }

        public ExcelColumnType Type { get; }

        public string Title { get; }

        public string Tooltip { get; set; }

        public short Width { get; set; }

        public short Height { get; set; }

        public string Format { get; set; }

        public HorizontalAlignment Align { get; set; }

        public VerticalAlignment VAlign { get; set; }

        public bool WrapText { get; set; }

        public bool IsBold { get; set; }

        public XSSFColor FontColor { get; set; }

        public int? FontSize { get; set; }

        public abstract void SetCellValue(ICell cell, TData data);


        protected readonly static XSSFColor _alternationColor = new XSSFColor(new byte[] { 240, 249, 235 });

        private readonly ICellStyle[] _cellStyles = new ICellStyle[(byte)ExcelColumnStyleNames.All + 1];
        public virtual ICellStyle CreateCellStyle(IWorkbook workbook, ExcelColumnStyleNames styleName)
        {
            if (workbook is null)
            {
                throw new ArgumentNullException(nameof(workbook));
            }
            byte styleIndex = (byte)styleName;
            if (_cellStyles[styleIndex] == null)
            {
                var style = (XSSFCellStyle)workbook.CreateCellStyle();
                if (!string.IsNullOrEmpty(Format))
                {
                    var dataFormat = workbook.CreateDataFormat();
                    style.DataFormat = dataFormat.GetFormat(Format);
                }
                style.Alignment = Align;
                style.VerticalAlignment = VAlign;
                style.WrapText = WrapText;
                if (styleName.HasFlag(ExcelColumnStyleNames.Alternation))
                {
                    style.FillForegroundXSSFColor = _alternationColor;
                    style.FillPattern = FillPattern.SolidForeground;
                }
                style.BorderBottom = BorderStyle.Thin;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderTop = BorderStyle.Thin;
                if (IsBold || FontColor != null || FontSize.HasValue || styleName.HasFlag(ExcelColumnStyleNames.Total))
                {
                    var font = (XSSFFont)workbook.CreateFont();
                    if (styleName.HasFlag(ExcelColumnStyleNames.Total))
                    {
                        font.IsBold = true;
                        font.FontHeightInPoints = 18;
                    }
                    else
                    {
                        font.IsBold = IsBold;
                        if (FontSize.HasValue)
                            font.FontHeightInPoints = FontSize.Value;
                    }

                    if (FontColor != null)
                        font.SetColor(FontColor);

                    style.SetFont(font);
                }
                _cellStyles[styleIndex] = style;
            }

            return _cellStyles[styleIndex];
        }
    }

    [Flags]
    public enum ExcelColumnStyleNames : byte
    {
        None = 0,
        Alternation = 1,
        Total = 2,
        All = 3,
    }
}
