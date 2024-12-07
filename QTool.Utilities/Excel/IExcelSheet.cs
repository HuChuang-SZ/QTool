using NPOI.SS.UserModel;
using System.Collections.Generic;

namespace QTool
{
    public interface IExcelSheet
    {
        void Export(IWorkbook workbook);
    }
}