using NPOI.SS.UserModel;
using QTool.Excel;
using System;
using System.Collections.Generic;

namespace QTool
{
    public class ExcelColumnInt32<TData> : ExcelColumn<TData>, IExcelTotalColumn<TData>
    {

        public ExcelColumnInt32(string title, Func<TData, int?> getValue, bool isTotal = false) : base(title, ExcelColumnType.Int32)
        {
            _getValue = getValue ?? throw new ArgumentNullException(nameof(getValue));
            IsTotal = isTotal;
            Format = "##0";
            Width = 120;
            Align = HorizontalAlignment.Right;
        }

        private readonly Func<TData, int?> _getValue;

        public override void SetCellValue(ICell cell, TData data)
        {
            SetCellValue(cell, _getValue(data));
        }

        private void SetCellValue(ICell cell, int? value)
        {
            cell.SetCellType(CellType.Numeric);

            if (value.HasValue)
                cell.SetCellValue(value.Value);
        }

        private int _countValue;

        public bool IsTotal { get; }

        public void Reset()
        {
            TotalRowCount = 0;
            _countValue = 0;
        }

        public int TotalRowCount { get; private set; }

        public void SetCellValue(ICell cell)
        {
            SetCellValue(cell, _countValue);
        }

        public void Sum(TData data)
        {
            TotalRowCount++;
            _countValue += _getValue(data) ?? 0;
        }
    }
}
