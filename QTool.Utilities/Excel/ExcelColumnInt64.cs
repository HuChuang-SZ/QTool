using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;

namespace QTool
{
    public class ExcelColumnInt64<TData> : ExcelColumn<TData>
    {
        public Func<TData, long?> GetValue { get; }

        public ExcelColumnInt64(string title, Func<TData, long?> getValue) : base(title, ExcelColumnType.Int64)
        {
            GetValue = getValue ?? throw new ArgumentNullException(nameof(getValue));
            Width = 200; 
            Align = HorizontalAlignment.Right;
        }

        public override void SetCellValue(ICell cell, TData data)
        {
            cell.SetCellType(CellType.String);
            var val = GetValue(data);
            if (val.HasValue)
                cell.SetCellValue(val.ToString());
        }
    }
}
