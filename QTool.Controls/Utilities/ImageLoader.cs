using Imazen.WebP;
using Microsoft.SqlServer.Server;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QTool.Controls.Utilities
{
    public static class ImageLoader
    {
        #region 静态方法
        private static readonly ConcurrentQueue<ImageLoadItem> _pendingQueue = new ConcurrentQueue<ImageLoadItem>();
        private static readonly ConcurrentDictionary<string, ImageLoadItem> _loadingDictionary = new ConcurrentDictionary<string, ImageLoadItem>();
        private static readonly Thread[] _downloadThreads;

        private static readonly string _imagePath;

        static ImageLoader()
        {
            var version = Environment.Version;
            if (version.Revision >= 42000)
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)(3072 | 768 | 192 | 48);
            else
            {
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)(192 | 48);
            }

            _imagePath = GlobalHelper.CreatePath("Temp", "Images");
            _downloadThreads = new Thread[10];
            for (int i = 0; i < _downloadThreads.Length; i++)
            {
                _downloadThreads[i] = new Thread(new ThreadStart(DoWork));
                _downloadThreads[i].IsBackground = true;
                _downloadThreads[i].Start();
            }
        }

        public static void Enqueue(ImageLoadItem item)
        {
            _pendingQueue.Enqueue(item);
        }


        private static void DoWork()
        {
            while (!AppHelper.IsExit)
            {
                if (_pendingQueue.TryDequeue(out ImageLoadItem item))
                {
                    LoadSingle(item);
                }
                else
                {
                    Thread.Sleep(500);
                }
            }
        }

        private static void LoadSingle(ImageLoadItem item)
        {
            if (_loadingDictionary.TryGetValue(item.ImageFile, out ImageLoadItem relatedItem))
            {
                relatedItem.RelatedItems.Add(item);
            }
            else
            {
                if (_loadingDictionary.TryAdd(item.ImageFile, item))
                {
                    item.OnLoad();
                    _loadingDictionary.TryRemove(item.ImageFile, out relatedItem);
                }
                else
                {
                    LoadSingle(item);
                }
            }
        }

        private static string DownloadFile(Uri uri)
        {
            string saveFile = Path.Combine(_imagePath, Guid.NewGuid().ToString() + ".tmp");
            DownloadFile(uri, saveFile);
            return saveFile;
        }

        private static void DownloadFile(Uri uri, string saveFile)
        {
            try
            {
                using (var client = new WebClient())
                {
                    client.Headers[HttpRequestHeader.UserAgent] = QRequestHeader.UserAgent;
                    client.DownloadFile(uri, saveFile);
                }
            }
            catch
            {
                throw;
            }
        }

        public static Image CreateImage(string imageFile, ImageFileFormat format)
        {
            try
            {
                if (format == ImageFileFormat.WEBP)
                {
                    var det = new SimpleDecoder();
                    var bytes = File.ReadAllBytes(imageFile);
                    return det.DecodeFromBytes(bytes, bytes.Length);
                }
                else
                {
                    using (var stream = new FileStream(imageFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        return Image.FromStream(stream);
                    }
                }
            }
            catch (DllNotFoundException)
            {
                throw;
            }
            catch
            {
                throw new InvalidImageFormatException();
            }
        }


        public static Image CreateImage(string imageFile)
        {
            return CreateImage(imageFile, GetImageFormat(imageFile));
        }

        public static ImageFileFormat GetImageFormat(string imageFile)
        {
            var buffer = new byte[12];
            int length;
            using (var fs = File.OpenRead(imageFile))
            {
                length = fs.Read(buffer, 0, buffer.Length);
            }

            if (length == buffer.Length)
            {
                switch (buffer[0])
                {
                    case 0xFF:
                        if (buffer[1] == 0xD8)
                            return ImageFileFormat.JPG;
                        break;
                    case 0x89:
                        if (buffer[1] == 0x50 && buffer[2] == 0x4E && buffer[3] == 0x47)
                            return ImageFileFormat.PNG;
                        break;
                    case 0x47:
                        if (buffer[1] == 0x49 && buffer[2] == 0x46)
                            return ImageFileFormat.GIF;
                        break;
                    case 0x0C:
                        if (buffer[1] == 0xED)
                            return ImageFileFormat.TIF;
                        break;
                    case 0x42:
                        if (buffer[1] == 0x4D)
                            return ImageFileFormat.BMP;
                        break;
                    case 0x52:
                        var str = Encoding.ASCII.GetString(buffer);
                        if (str.IndexOf("RIFF") == 0 && str.IndexOf("WEBP") == 8)
                            return ImageFileFormat.WEBP;
                        break;
                    default:
                        break;
                }
            }

            return ImageFileFormat.Unkown;
        }

        public static Size GetIamgeSize(string imageFile)
        {
            var image = new System.Windows.Media.Imaging.BitmapImage();
            using (var stream = File.OpenRead(imageFile))
            {
                image.BeginInit();
                image.StreamSource = stream;
                image.EndInit();
                int width = image.PixelWidth;
                int height = image.PixelHeight;
                return new Size(width, height);
            }
        }

        public static string GetTempFile(string uriString)
        {
            if (!TryCreateUri(uriString, out Uri uri))
                throw new ArgumentOutOfRangeException(nameof(uriString));

            using (MD5 md5 = new MD5CryptoServiceProvider())
            {
                byte[] buffer;
                if (uri.IsFile && File.Exists(uri.LocalPath))
                {
                    buffer = File.ReadAllBytes(uri.LocalPath);
                }
                else
                {
                    buffer = Encoding.UTF8.GetBytes(uriString);
                }

                byte[] bytes = md5.ComputeHash(buffer);

                const int folderLevel = 2;
                var paths = new string[folderLevel + 1];
                paths[0] = _imagePath;
                for (int i = 0; i < folderLevel; i++)
                {
                    paths[i + 1] = bytes[i].ToString("x2");
                }

                var path = Path.Combine(paths);

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                var extension = Path.GetExtension(uri.LocalPath);

                if (string.IsNullOrEmpty(extension))
                    extension = ".tmp";

                return Path.Combine(path, string.Concat(bytes.Skip(folderLevel).Select(b => b.ToString("x2"))) + extension);
            }
        }

        public static bool TryDownloadFile(string uriString, string imageFile)
        {
            if (TryCreateUri(uriString, out Uri uri))
            {
                if (File.Exists(imageFile))
                {
                    return true;
                }
                else if (uri.IsFile)
                {
                    if (File.Exists(uri.LocalPath))
                    {
                        File.Copy(uri.LocalPath, imageFile);
                        return true;
                    }
                    else
                    {
                        throw new FileNotFoundException("未找到文件", uri.LocalPath);
                    }
                }
                else
                {
                    string tempFile = DownloadFile(uri);
                    File.Move(tempFile, imageFile);
                    return true;
                }
            }
            else
            {
                throw new Exception($"无效的图片链接：{uriString}。");
            }
        }

        public static bool TryDownloadFile(string uriString, out string imageFile)
        {
            imageFile = GetTempFile(uriString);
            return TryDownloadFile(uriString, imageFile);
        }

        public static bool TryCreateUri(string uriString, out Uri uri)
        {
            if (string.IsNullOrWhiteSpace(uriString))
                throw new ArgumentNullException(nameof(uriString));

            if (uriString.IndexOf("//") == 0)
            {
                uriString = "http:" + uriString;
            }
            return Uri.TryCreate(uriString, UriKind.Absolute, out uri);
        }

        public static Size CalcZoomSize(Size sourceSize, Size zoomSize)
        {
            if (sourceSize.Width * 1.0 / sourceSize.Height > zoomSize.Width * 1.0 / zoomSize.Height)
            {
                var height = sourceSize.Height * zoomSize.Width * 1.0 / sourceSize.Width;
                return new Size(zoomSize.Width, (int)height);
            }
            else
            {
                var width = sourceSize.Width * zoomSize.Height * 1.0 / sourceSize.Height;
                return new Size((int)width, zoomSize.Height);
            }
        }

        public static void Zoom(string sourceFile, Size thumbnailSize, string thumbnailFile)
        {
            using (var sourceImage = CreateImage(sourceFile))
            {
                Zoom(sourceImage, thumbnailSize, thumbnailFile);
            }
        }

        public static void Zoom(Image sourceImage, Size thumbnailSize, string thumbnailFile)
        {
            Size size = CalcZoomSize(sourceImage.Size, thumbnailSize);
            using (var thumbnailImg = sourceImage.GetThumbnailImage(size.Width, size.Height, null, IntPtr.Zero))
            {
                thumbnailImg.Save(thumbnailFile);
            }
        }
        #endregion 静态方法

        public static bool TryGetImageFile(this Dictionary<string, ImageLoadItem> imageMaps, string imageUri, out string imageFile)
        {
            if (imageMaps.TryGetValue(imageUri, out ImageLoadItem imageItem))
            {
                while (imageItem.Status == ImageLoadStatus.Pending)
                {
                    Task.Delay(100).Wait();
                }

                if (imageItem.Status == ImageLoadStatus.Completed)
                {
                    imageFile = imageItem.ImageFile;
                    return true;
                }
            }
            imageFile = null;
            return false;
        }
    }
}
