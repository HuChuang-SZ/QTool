using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;

namespace QTool
{
    public class ExcelColumnString<TData> : ExcelColumn<TData>
    {

        public ExcelColumnString(string title, Func<TData, string> getValue) : base(title, ExcelColumnType.String)
        {
            GetValue = getValue ?? throw new ArgumentNullException(nameof(getValue));
            Width = 200;
            Align = HorizontalAlignment.Left;
        }

        public Func<TData, string> GetValue { get; }

        public override void SetCellValue(ICell cell, TData data)
        {
            cell.SetCellType(CellType.String);
            cell.SetCellValue(GetValue(data));
        }
    }
}
