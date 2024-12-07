using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;

namespace QTool
{
    public class ExcelColumnNumeric<TData> : ExcelColumn<TData>
    {

        public ExcelColumnNumeric(string title, Func<TData, decimal?> getValue) : base(title, ExcelColumnType.Numeric)
        {
            GetValue = getValue ?? throw new ArgumentNullException(nameof(getValue));
            Width = 120;
            Align = HorizontalAlignment.Right;
        }

        public Func<TData, decimal?> GetValue { get; }


        public override void SetCellValue(ICell cell, TData data)
        {
            cell.SetCellType(CellType.Numeric);
            var val = GetValue(data);
            if (val.HasValue)
            {
                cell.SetCellValue((double)val.Value);
            }
        }
    }
}
