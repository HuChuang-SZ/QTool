using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;

namespace QTool
{
    public class ExcelColumnBoolean<TData> : ExcelColumn<TData>
    {

        public ExcelColumnBoolean(string title, Func<TData, bool?> getValue) : base(title, ExcelColumnType.Boolean)
        {
            GetValue = getValue ?? throw new ArgumentNullException(nameof(getValue));
            Width = 200;
            Align = HorizontalAlignment.Left;
        }

        public Func<TData, bool?> GetValue { get; }


        public override void SetCellValue(ICell cell, TData data)
        {
            cell.SetCellType(CellType.Boolean);
            var val = GetValue(data);
            if (val.HasValue)
                cell.SetCellValue(val.Value);
        }
    }
}
