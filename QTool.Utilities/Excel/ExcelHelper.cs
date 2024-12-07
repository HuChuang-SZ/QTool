using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTool
{
    public static class ExcelHelper
    {
        public static void Export(this Stream stream, params IExcelSheet[] sheets)
        {
            var workbook = new XSSFWorkbook();
            Dictionary<string, int> pictureMap = new Dictionary<string, int>();

            foreach (var sheet in sheets)
            {
                sheet.Export(workbook);
            }

            workbook.Write(stream);
            workbook.Close();
        }

        public static short ToRowHeight(this short pixel)
        {
            return (short)(pixel * 15);
        }

        public static short ToRowHeight(this int pixel)
        {
            return (short)(pixel * 15);
        }
    }
}
