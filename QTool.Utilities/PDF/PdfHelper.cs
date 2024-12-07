using iTextSharp.text.pdf;
using iTextSharp.text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace QTool.PDF
{

    public static class PdfHelper
    {

        //public byte[] CreatePdf(Rectangle pageSize)
        //{
        //    string fileName = GetLocalPath();
        //    try
        //    {

        //        #region 生成pdf文件
        //        using (FileStream fs = new FileStream(fileName, FileMode.Create))
        //        {
        //            using (Document document = new Document(pageSize, 10, 10, 10, 10))
        //            {
        //                using (PdfWriter writer = PdfWriter.GetInstance(document, fs))
        //                {
        //                    document.Open();
        //                    try
        //                    {

        //                    }
        //                    finally
        //                    {
        //                        document.Close();
        //                        fs.Close();
        //                    }
        //                }
        //            }
        //        }
        //        #endregion
        //    }
        //    finally
        //    {

        //    }
        //}

        public static BaseFont Font { get; } = BaseFont.CreateFont(Path.Combine(GlobalHelper.RootPath, @"msyh.ttf"), BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

        public static string GetTempFile()
        {
            string tempPath = GlobalHelper.CreateTempPath(GlobalHelper.TempPath, "PDF");
            if (!Directory.Exists(tempPath)) Directory.CreateDirectory(tempPath);

            return Path.Combine(tempPath, Guid.NewGuid().ToString() + ".pdf");
        }

        public static Image CreateBarcode128(this PdfWriter writer, string code, int width, int height, PTextAlignment alignment)
        {
            PdfContentByte cd = writer.DirectContent;
            Barcode128 code128 = new Barcode128();
            code128.CodeType = Barcode.CODE128;
            code128.Code = code;
            code128.Font = null;
            float ratio = 1;
            ratio = width / code128.BarcodeSize.Width;
            code128.BarHeight = height / ratio;
            Image img = code128.CreateImageWithBarcode(cd, null, null);
            img.ScaleAbsolute(img.Width * ratio, img.Height * ratio);
            img.Alignment = (int)alignment;
            return img;
        }

        public static Image CreateBarcodeInter25(this PdfWriter writer, string code, int width, int height, PTextAlignment alignment, float barN = 2.0f)
        {
            PdfContentByte cd = writer.DirectContent;
            Barcode barcode = new BarcodeInter25();
            barcode.Code = code;
            barcode.Font = null;
            barcode.N = barN;
            float ratio = 1;
            if (width > barcode.BarcodeSize.Width)
            {
                ratio = width / barcode.BarcodeSize.Width;
            }
            barcode.BarHeight = height / ratio;
            Image img = barcode.CreateImageWithBarcode(cd, null, null);
            img.ScaleAbsolute(img.Width * ratio, img.Height * ratio);
            img.Alignment = (int)alignment;
            return img;
        }

        #region CreateParagraph
        public static Paragraph CreateParagraph(PTextAlignment alignment, params Chunk[] chunks)
        {
            Paragraph paragraph = new Paragraph
            {
                Alignment = (int)alignment
            };
            float size = 0f;
            foreach (Chunk chunk in chunks)
            {
                if (chunk.Font.Size > size)
                {
                    size = chunk.Font.Size;
                }
                paragraph.Add(chunk);
            }
            paragraph.Leading = size * 1.2f;
            return paragraph;
        }

        public static Paragraph CreateParagraph(string text, float fontSize = 9, PFontStyle style = PFontStyle.Normal, string color = "#000000", PTextAlignment alignment = PTextAlignment.Left)
        {
            return CreateParagraph(text, fontSize, fontSize * 1.2f, style, color, alignment);
        }
        public static Paragraph CreateParagraph(string text, float fontSize, float leading, PFontStyle style = PFontStyle.Normal, string color = "#000000", PTextAlignment alignment = PTextAlignment.Left)
        {
            return new Paragraph(leading, text, new Font(Font, fontSize, (int)style, CreateColor(color))) { Alignment = (int)alignment };
        }

        #endregion CreateParagraph

        public static void AddParagraph(this PdfPCell cell, string text, float fontSize = 9, PFontStyle style = PFontStyle.Normal, string color = "#000000", PTextAlignment alignment = PTextAlignment.Left)
        {
            cell.AddElement(CreateParagraph(text, fontSize, style, color, alignment));
        }

        public static BaseColor CreateColor(string color)
        {
            return new BaseColor(System.Drawing.ColorTranslator.FromHtml(color));
        }

        #region CreateChunk
        public static Chunk GetOldChunk(string text, BaseFont fontFamily, float fontSize, PFontStyle fontStyle)
        {
            return new Chunk(text, new Font(fontFamily, fontSize, (int)fontStyle));
        }

        public static Chunk GetOldChunk(string text, float fontSize = 9, PFontStyle fontStyle = PFontStyle.Normal)
        {
            return GetOldChunk(text, Font, fontSize, fontStyle);
        }

        public static Chunk CreateChunk(string text, float fontSize = 9, PFontStyle fontStyle = PFontStyle.Normal)
        {
            return new Chunk(text, new Font(Font, fontSize, (int)fontStyle));
        }
        #endregion CreateChunk

        #region CreateImage
        private static Image CreateImage(string imageFile, float width, float height)
        {
            Image img = Image.GetInstance(imageFile);
            img.Alignment = Element.ALIGN_CENTER;
            img.ScaleAbsolute(width, height);
            return img;
        }

        public static Image CreateImage(string imgName, float maxValue, PTextAlignment alignment)
        {
            if (File.Exists(imgName))
            {
                Image img = Image.GetInstance(imgName);
                img.Alignment = (int)alignment;

                System.Drawing.SizeF size = new System.Drawing.SizeF(img.PlainWidth, img.PlainHeight).CalcEqualRatioSize(maxValue);

                img.ScaleAbsolute(size.Width, size.Height);
                return img;
            }
            else
            {
                return null;
            }
        }

        public static System.Drawing.SizeF CalcEqualRatioSize(this System.Drawing.SizeF size, float maxValue)
        {
            if (size.Width > size.Height)
            {
                return new System.Drawing.SizeF(maxValue, size.Height * maxValue / size.Width);
            }
            else
            {
                return new System.Drawing.SizeF(size.Width * maxValue / size.Height, maxValue);
            }
        }
        #endregion CreateImage

        public static PdfPCell CreateTableCell(PBorder boder = PBorder.RightBottom, int colspan = 1, int rowspan = 1, int paddingTop = 0)
        {
            if (colspan <= 0) throw new ArgumentNullException("colspan");
            if (rowspan <= 0) throw new ArgumentNullException("rowspan");

            PdfPCell cell = new PdfPCell();
            cell.Rowspan = rowspan;
            cell.Colspan = colspan;
            cell.Border = (byte)boder;
            cell.Padding = 3;
            cell.PaddingTop = paddingTop;
            return cell;
        }

        public static string GetAreaCodeByPostCode(string postCode)
        {
            int result = 0;
            int.TryParse(postCode, out result);
            if (result >= 100000 && result < 200000)
                return "1";
            else if (result >= 20000 && result < 300000)
                return "2";
            else if (result >= 300000 && result < 400000)
                return "3";
            else if (result >= 400000 && result < 500000)
                return "4";
            else if (result >= 500000 && result < 600000)
                return "0";
            else if (result >= 600000 && result < 630000)
                return "4";
            else if (result >= 630000 && result < 639999)
                return "0";
            else if (result >= 639999 && result < 642999)
                return "4";
            else if (result >= 642999 && result < 700000)
                return "6";
            else if (result > 700000)
                return "6";
            else
                return null;
        }
    }
}
