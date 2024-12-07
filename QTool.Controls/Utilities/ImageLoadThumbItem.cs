using Newtonsoft.Json;

using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.IO;
using System.Text;

namespace QTool.Controls.Utilities
{
    public class ImageLoadThumbItem : ImageLoadItem
    {
        private readonly Action<ImageLoadThumbItem> _callback;

        private readonly string _configFile;
        private Size _imageSize = Size.Empty;

        public ImageLoadThumbItem(string uriString, int thumbnailValue = 0, Action<ImageLoadThumbItem> callback = null)
            : base(uriString)
        {
            if (Status != ImageLoadStatus.Failed)
            {
                try
                {
                    _callback = callback;
                    _configFile = Path.ChangeExtension(ImageFile, ".ini");
                    if (thumbnailValue > 0)
                    {
                        ThumbnailValue = thumbnailValue;
                        ThumbnailImage = string.Join("_", ImageFile, ThumbnailValue);
                    }
                    else
                    {
                        ThumbnailValue = 0;
                        ThumbnailImage = ImageFile;
                    }
                }
                catch (Exception ex)
                {
                    LoadFailed(ex);
                }
            }
        }

        /// <summary>
        /// 图片格式
        /// </summary>
        public ImageFileFormat Format { get; private set; }

        /// <summary>
        /// 图片宽度
        /// </summary>
        public int Width { get { return _imageSize.Width; } }

        /// <summary>
        /// 图片高度
        /// </summary>
        public int Height { get { return _imageSize.Height; } }

        /// <summary>
        /// 缩略图文件
        /// </summary>
        public string ThumbnailImage { get; }

        /// <summary>
        /// 缩略值
        /// </summary>
        public int ThumbnailValue { get; }

        protected override void LoadCompleted()
        {
            base.LoadCompleted();
            Format = ImageLoader.GetImageFormat(ImageFile);
            _imageSize = GetIamgeSize();
            Thumbnail();
        }

        protected override void ClearHistoryFile()
        {
            base.ClearHistoryFile();
            FileHelper.Delete(ThumbnailImage);
            FileHelper.Delete(_configFile);
        }

        /// <summary>
        /// 回调
        /// </summary>
        protected override void OnCallback()
        {
            base.OnCallback();
            _callback?.Invoke(this);
        }

        private void Thumbnail()
        {
            if (!File.Exists(ThumbnailImage))
            {
                var thumbnnailImgSize = GetThumbnailImgSize(_imageSize);
                if (thumbnnailImgSize != _imageSize)
                {
                    using (Image img = ImageLoader.CreateImage(ImageFile, Format))
                    {
                        ImageLoader.Zoom(img, thumbnnailImgSize, ThumbnailImage);
                    }
                }
                else
                {
                    File.Copy(ImageFile, ThumbnailImage);
                }
            }
        }


        private Size GetIamgeSize()
        {
            Size size;
            if (File.Exists(_configFile))
            {
                try
                {
                    size = JsonConvert.DeserializeObject<Size>(File.ReadAllText(_configFile, Encoding.UTF8));
                    if (!size.IsEmpty)
                    {
                        return size;
                    }
                }
                catch
                {
                    FileHelper.Delete(_configFile);
                }
            }

            size = ImageLoader.GetIamgeSize(ImageFile);
            File.WriteAllText(_configFile, JsonConvert.SerializeObject(size));
            return size;
        }

        private Size GetThumbnailImgSize(Size imageSize)
        {
            int width, height;
            if (imageSize.Width > ThumbnailValue || imageSize.Height > ThumbnailValue)
            {
                double max = Math.Max(imageSize.Width * 1.0 / ThumbnailValue, imageSize.Height * 1.0 / ThumbnailValue);
                width = (int)(imageSize.Width / max);
                height = (int)(imageSize.Height / max);
            }
            else
            {
                width = imageSize.Width;
                height = imageSize.Height;
            }

            return new Size(width, height);
        }

        public bool IsMatch(string uriSource, int thumbnailValue)
        {
            return ThumbnailValue == thumbnailValue && UriString == uriSource ;
        }
    }
}
