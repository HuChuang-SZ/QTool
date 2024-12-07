using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTool.Excel
{
    public interface IExcelTotalColumn<TData>
    {
        bool IsTotal { get; }

       int TotalRowCount { get; }

        void Reset();

        void Sum(TData data);

        void SetCellValue(ICell cell);
    }
}
