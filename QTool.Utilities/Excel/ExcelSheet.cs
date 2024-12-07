using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using QTool.Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;

namespace QTool
{
    public class ExcelSheet<TData> : IExcelSheet
    {

        private readonly Func<TData, string> _getGroupId;
        public ExcelSheet(string sheetName, ExcelColumn<TData>[] columns, IList<TData> datas, Func<TData, string> getGroupId = null)
        {
            if (string.IsNullOrWhiteSpace(sheetName)) throw new ArgumentNullException(nameof(sheetName));
            if (columns == null || columns.Length == 0) throw new ArgumentNullException(nameof(columns));
            if (datas == null || datas.Count == 0) throw new ArgumentNullException(nameof(datas));

            SheetName = sheetName;
            Columns = columns;
            Datas = datas;
            _getGroupId = getGroupId;
            RowHeight = Columns.Max(c => c.Height).ToRowHeight();
        }


        #region SheetName Property
        private string _sheetName;
        /// <summary>
        /// 名称
        /// </summary>
        public string SheetName
        {
            get
            {
                return _sheetName;
            }
            set
            {
                if (_sheetName != value)
                {
                    _sheetName = value;
                }
            }
        }
        #endregion SheetName Property

        #region RowHeight Property
        private short _rowHeight = 19;
        /// <summary>
        /// 行高
        /// </summary>
        public short RowHeight
        {
            get
            {
                return _rowHeight;
            }
            set
            {
                if (_rowHeight != value)
                {
                    _rowHeight = value;
                }
            }
        }
        #endregion RowHeight Property

        public ExcelColumn<TData>[] Columns { get; }

        public IList<TData> Datas { get; }


        public void Export(IWorkbook workbook)
        {
            if (workbook == null) throw new ArgumentNullException(nameof(workbook));

            ISheet sheet = workbook.CreateSheet(string.IsNullOrWhiteSpace(SheetName) ? "Sheet" : SheetName);
            int rowIndex = 0;

            CreateHeaderRow(workbook, sheet);
            rowIndex++;
            var totalColumns = new List<IExcelTotalColumn<TData>>();
            foreach (var column in Columns)
            {
                if (column is ExcelColumnImage<TData> columnImage)
                {
                    columnImage.LoadImage(workbook, Datas);
                }

                if (TryGetTotalColumn(column, out IExcelTotalColumn<TData> totalColumn))
                {
                    totalColumns.Add(totalColumn);
                }
            }

            string lastGroupId = null;
            int groupIndex = 0;
            for (int i = 0; i < Datas.Count; i++)
            {
                var data = Datas[i];
                var groupId = _getGroupId?.Invoke(data) ?? string.Empty;
                if (lastGroupId == null)
                {
                    lastGroupId = groupId;
                }
                else if (lastGroupId != groupId)
                {
                    if (TryExportTotalRow(workbook, sheet, rowIndex, totalColumns, GetStyleName(groupIndex)))
                    {
                        rowIndex++;
                    }
                    lastGroupId = groupId;
                    groupIndex++;
                }

                if (totalColumns.Count > 0)
                {
                    foreach (var column in totalColumns)
                    {
                        column.Sum(data);
                    }
                }
                ExprotRow(workbook, sheet.CreateRow(rowIndex++), data, GetStyleName(groupIndex));
            }

            TryExportTotalRow(workbook, sheet, rowIndex, totalColumns, GetStyleName(groupIndex));

            for (int columnIndex = 0; columnIndex < Columns.Length; columnIndex++)
            {
                if (Columns[columnIndex].Width > 0)
                    sheet.SetColumnWidth(columnIndex, Columns[columnIndex].Width * 32);
            }
        }

        private ExcelColumnStyleNames GetStyleName(int groupIndex)
        {
            if (groupIndex % 2 == 1)
            {
                return ExcelColumnStyleNames.Alternation;
            }
            else
            {
                return ExcelColumnStyleNames.None;
            }
        }

        private void ExprotRow(IWorkbook workbook, IRow row, TData data, ExcelColumnStyleNames styleName)
        {
            if (RowHeight > 0)
                row.Height = RowHeight;

            ICell cell;
            for (int columnIndex = 0; columnIndex < Columns.Length; columnIndex++)
            {
                cell = row.CreateCell(columnIndex);
                cell.CellStyle = Columns[columnIndex].CreateCellStyle(workbook, styleName);
                Columns[columnIndex].SetCellValue(cell, data);
            }
        }

        private bool TryExportTotalRow(IWorkbook workbook, ISheet sheet, int rowIndex, List<IExcelTotalColumn<TData>> totalColumns, ExcelColumnStyleNames styleName)
        {
            if (totalColumns.Count > 0)
            {
                var result = totalColumns[0].TotalRowCount > 1;
                if (result)
                {
                    var row = sheet.CreateRow(rowIndex);
                    row.Height = 30.ToRowHeight();
                    ICell cell;
                    for (int columnIndex = 0; columnIndex < Columns.Length; columnIndex++)
                    {
                        cell = row.CreateCell(columnIndex);
                        cell.CellStyle = Columns[columnIndex].CreateCellStyle(workbook, styleName | ExcelColumnStyleNames.Total);
                        if (TryGetTotalColumn(Columns[columnIndex], out IExcelTotalColumn<TData> totalColumn))
                        {
                            totalColumn.SetCellValue(cell);
                        }
                    }
                }

                foreach (var totalColumn in totalColumns)
                {
                    totalColumn.Reset();
                }
                return result;
            }
            else
            {
                return false;
            }
        }

        private static bool TryGetTotalColumn(ExcelColumn<TData> column, out IExcelTotalColumn<TData> totalColumn)
        {
            totalColumn = column as IExcelTotalColumn<TData>;
            return totalColumn != null && totalColumn.IsTotal;
        }

        private void CreateHeaderRow(IWorkbook workbook, ISheet sheet)
        {
            //创建列头
            IRow row = sheet.CreateRow(0);
            ICellStyle headerStyle = CreateHeaderCellStyle(workbook);
            for (int columnIndex = 0; columnIndex < Columns.Length; columnIndex++)
            {
                ICell cell = row.CreateCell(columnIndex, CellType.String);
                cell.SetCellValue(Columns[columnIndex].Title);
                cell.CellStyle = headerStyle;
            }
        }

        private ICellStyle CreateHeaderCellStyle(IWorkbook workbook)
        {
            ICellStyle style = workbook.CreateCellStyle();

            IFont font = workbook.CreateFont();
            font.IsBold = true;

            style.SetFont(font);
            style.VerticalAlignment = VerticalAlignment.Center;
            style.Alignment = HorizontalAlignment.Center;

            return style;
        }
    }
}
