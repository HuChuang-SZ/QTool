using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;

namespace QTool
{
    public class ExcelColumnDate<TData> : ExcelColumn<TData>
    {

        public ExcelColumnDate(string title, Func<TData, DateTime?> getValue) : base(title, ExcelColumnType.DataTime)
        {
            GetValue = getValue ?? throw new ArgumentNullException(nameof(getValue));
            Format = "yyyy-MM-dd HH:mm:ss";
            Width = 160;
            Align = HorizontalAlignment.Center;
        }

        public Func<TData, DateTime?> GetValue { get; }


        public override void SetCellValue(ICell cell, TData data)
        {
            cell.SetCellType(CellType.Numeric);
            var dataVal = GetValue(data);
            if (dataVal.HasValue)
                cell.SetCellValue(dataVal.Value);
        }
    }
}
