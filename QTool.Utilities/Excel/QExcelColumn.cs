using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTool.Excel
{
    public abstract class QExcelColumn
    {
        public string Name { get; set; }

        public ICellStyle CellStyle { get; set; } 
    }
}
