using NPOI.SS.UserModel;
using NPOI.Util;
using System;
using System.Collections.Generic;
using System.IO;

namespace QTool
{
    public class ExcelColumnImage<TData> : ExcelColumn<TData>
    {

        private readonly Dictionary<string, int> _pictureMaps = new Dictionary<string, int>();
        public ExcelColumnImage(string title, Func<TData, string> getValue) : base(title, ExcelColumnType.Image)
        {
            GetValue = getValue ?? throw new ArgumentNullException(nameof(getValue));
            Align = HorizontalAlignment.Center;
            Height = 66;
            Width = 66;
        }

        public Func<TData, string> GetValue { get; }

        public override void SetCellValue(ICell cell, TData data)
        {
            string imgUri = GetValue(data);
            if (!string.IsNullOrWhiteSpace(imgUri))
            {
                if (_pictureMaps.TryGetValue(imgUri, out int imgIndex))
                {
                    IDrawing patriarch = cell.Sheet.CreateDrawingPatriarch();
                    int padding = 3 * Units.EMU_PER_POINT;
                    var anchor = patriarch.CreateAnchor(padding, padding, -padding, -padding, cell.ColumnIndex, cell.RowIndex, cell.ColumnIndex + 1, cell.RowIndex + 1);
                    anchor.AnchorType = 0;
                    patriarch.CreatePicture(anchor, imgIndex);
                }
            }
        }

        public void LoadImage(IWorkbook workbook, IList<TData> datas)
        {
            List<string> imageUris = new List<string>();
            foreach (var data in datas)
            {
                string uriString = GetValue(data);
                if (!string.IsNullOrWhiteSpace(uriString) && !imageUris.Contains(uriString))
                {
                    imageUris.Add(uriString);
                }
            }

            var items = Downloader.BatchDownloadFiles(imageUris.ToArray()).Result;

            foreach (var item in items)
            {
                if (!item.HasError
                    && !string.IsNullOrWhiteSpace(item.File)
                    && File.Exists(item.File))
                {
                    if (!_pictureMaps.ContainsKey(item.UriString))
                    {
                        byte[] bytes = File.ReadAllBytes(item.File);
                        var fileType = ImageHelper.GetImageFormat(bytes);

                        PictureType format;
                        switch (fileType)
                        {
                            case ImageFileFormat.JPG:
                                format = PictureType.JPEG;
                                break;
                            case ImageFileFormat.GIF:
                                format = PictureType.GIF;
                                break;
                            case ImageFileFormat.PNG:
                                format = PictureType.PNG;
                                break;
                            case ImageFileFormat.BMP:
                                format = PictureType.BMP;
                                break;
                            default:
                                continue;
                        }
                        int pictureIndex = workbook.AddPicture(bytes, format);
                        _pictureMaps[item.UriString] = pictureIndex;
                    }
                }
            }
        }
    }
}
