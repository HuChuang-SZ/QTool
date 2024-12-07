using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTool.PDF
{
    public abstract class BaseBuilder
    {

        /// <summary>
        /// 纸张宽度(px)
        /// </summary>
        public PSizeF PaperSize { get; }


        public PThicknessF Margin { get; }


        public BaseBuilder(PSizeF paperSize, PThicknessF margin)
        {
            PaperSize = paperSize;
            Margin = margin;
        }

        public string Build(Action<double> updateProgress)
        {
            string fileName = PdfHelper.GetTempFile();
            using (FileStream fs = new FileStream(fileName, FileMode.Create))
            {
                using (Document document = new Document(new Rectangle(PaperSize.Width, PaperSize.Height), Margin.Left, Margin.Right, Margin.Top, Margin.Bottom))
                {
                    using (PdfWriter writer = PdfWriter.GetInstance(document, fs))
                    {
                        document.Open();
                        try
                        {
                            Build(document, writer, updateProgress);
                        }
                        finally
                        {
                            document.Close();
                            fs.Close();
                        }
                    }
                }
            }
            return fileName;
        }

        protected abstract void Build(Document document, PdfWriter writer, Action<double> updateProgress);
    }
}
