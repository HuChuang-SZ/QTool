using CefSharp;
using CefSharp.Web;
using CefSharp.Wpf;
using HtmlAgilityPack;
using QTool.Controls;
using QTool.Controls.Utilities;
using QTool.View.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace QTool.View.ViewModels
{
    public class ImageDownloaderViewModel : INotifyPropertyChanged
    {
        public ImageDownloaderViewModel(Window owner, string html)
        {
            Owner = owner ?? throw new ArgumentNullException(nameof(owner));
            LoadImages(html);
        }

        public Window Owner { get; }

        //private string[] _imageExtensions = { ".jpg", ".jpge", ".png", ".gif", ".bmp" };
        private void LoadImages(string html)
        {
            IsWaiting = true;
            AsyncHelper.Exec(() =>
            {
                WaitMsg = "正在加载图片...";
                HtmlDocument document = new HtmlDocument();
                document.LoadHtml(html);
                var nodes = document.DocumentNode.GetElementByTagName("img");
                List<ImageLoadThumbItem> imageItems = new List<ImageLoadThumbItem>();
                foreach (var node in nodes)
                {
                    var imageUri = node.GetAttributeValue("src", string.Empty);
                    if (!string.IsNullOrEmpty(imageUri))
                    {
                        imageUri = Regex.Replace(imageUri, ".(jpg)_\\d+x\\d+.(jpg)", ".jpg");
                        var item = new ImageLoadThumbItem(imageUri, 120);
                        ImageLoader.Enqueue(item);
                        imageItems.Add(item);
                        WaitMsg = $"正在加载图片...{imageItems.Count}";
                    }
                }
                int qty = 0;
                do
                {
                    Task.Delay(500).Wait();
                    qty = imageItems.Count(i => i.Status == ImageLoadStatus.Pending);
                    WaitMsg = $"正在加载图片...{qty}";
                }
                while (qty > 0);

                return imageItems.Where(i => i.Status == ImageLoadStatus.Completed).Select(i => new ImageItem(this, i)).ToArray();
            }, result =>
            {
                if (result.HasError)
                {
                    QMessageBox.Show(result.Error.Message, Owner);
                }
                else
                {
                    Images = new ReadOnlyCollection<ImageItem>(result.Result);
                }

                IsWaiting = false;
            });
        }

        public int[] ImageFilterSizes { get; } = new int[] { 0, 200, 400, 600, 800 };

        #region FilterSize Property
        private int _filterSize = 400;
        /// <summary>
        /// 筛选尺寸
        /// </summary>
        public int FilterSize
        {
            get
            {
                return _filterSize;
            }
            set
            {
                if (_filterSize != value)
                {
                    _filterSize = value;
                    OnPropertyChanged(nameof(FilterSize));
                    RefreshImages();
                }
            }
        }
        #endregion FilterSize Property

        #region INotifyPropertyChanged
        [NonSerialized]
        private PropertyChangedEventHandler _propertyChanged;
        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                _propertyChanged += value;
            }
            remove
            {
                _propertyChanged -= value;
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            _propertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion INotifyPropertyChanged

        #region IsWaiting Property
        private bool _isWaiting;
        /// <summary>
        /// 是否等待中
        /// </summary>
        public bool IsWaiting
        {
            get
            {
                return _isWaiting;
            }
            set
            {
                if (_isWaiting != value)
                {
                    _isWaiting = value;
                    OnPropertyChanged(nameof(IsWaiting));
                }
            }
        }
        #endregion IsWaiting Property

        #region WaitMsg Property
        private string _waitMsg;
        /// <summary>
        /// 等待消息
        /// </summary>
        public string WaitMsg
        {
            get
            {
                return _waitMsg;
            }
            set
            {
                if (_waitMsg != value)
                {
                    _waitMsg = value;
                    OnPropertyChanged(nameof(WaitMsg));
                }
            }
        }
        #endregion WaitMsg Property

        #region Images Property
        private ReadOnlyCollection<ImageItem> _images;

        /// <summary>
        /// describe
        /// </summary>
        public ReadOnlyCollection<ImageItem> Images
        {
            get
            {
                return _images;
            }
            private set
            {
                if (_images != value)
                {
                    _images = value;
                    OnPropertyChanged(nameof(Images));
                    RefreshImages();
                }
            }
        }
        #endregion Images Property

        #region FilterImages Property
        private ReadOnlyCollection<ImageItem> _filterImages;
        /// <summary>
        /// 筛选图片
        /// </summary>
        public ReadOnlyCollection<ImageItem> FilterImages
        {
            get
            {
                return _filterImages;
            }
            private set
            {
                if (_filterImages != value)
                {
                    _filterImages = value;
                    OnPropertyChanged(nameof(FilterImages));
                }
            }
        }
        #endregion FilterImages Property

        private void RefreshImages()
        {
            var images = Images;
            var filterSize = FilterSize;
            if (images != null)
            {
                if (filterSize == 0)
                {
                    FilterImages = Images;
                }
                else
                {
                    AsyncHelper.Exec(() =>
                    {
                        return images?.Where(i => i.Width >= filterSize || i.Height >= filterSize).ToArray();
                    }, result =>
                    {
                        FilterImages = new ReadOnlyCollection<ImageItem>(result.Result);
                    });
                }
            }
            else
            {
                FilterImages = null;
            }
        }

        internal void DownloadImages(ImageItem[] images)
        {
            IsWaiting = true;
            AsyncHelper.Exec(() =>
            {
                string path = GetDownloadPath();
                int index = 0;
                foreach (var item in images)
                {
                    index++;
                    WaitMsg = $"正在下载第{index}/{images.Length}张图片...";
                    File.Copy(item.ImageFile, Path.Combine(path, index.ToString("0000") + "." + item.Format.ToString()));
                }
                return path;
            }, result =>
            {
                if (result.HasError)
                {
                    QMessageWindow.Show(result.Error.Message);
                }
                else
                {
                    result.Result.Start();
                }
                IsWaiting = false;
            });
        }

        private string GetDownloadPath()
        {
            string identity = DateTime.Now.ToString("yyyyMMdd-HHmmss-FFFFFF");
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", identity);
            if (Directory.Exists(path))
            {
                return GetDownloadPath();
            }
            else
            {
                Directory.CreateDirectory(path);
                return path;
            }
        }

        public class ImageItem : INotifyPropertyChanged
        {
            public ImageItem(ImageDownloaderViewModel owner, ImageLoadThumbItem item)
            {
                if (item is null)
                {
                    throw new ArgumentNullException(nameof(item));
                }

                Owner = owner ?? throw new ArgumentNullException(nameof(owner));

                ImageUri = item.UriString;
                ImageFile = item.ImageFile;
                ThumbnailImage = item.ThumbnailImage;
                Width = item.Width;
                Height = item.Height;
                Format = item.Format;
            }
            public ImageDownloaderViewModel Owner { get; }


            public string ImageUri { get; }

            public string ImageFile { get; }

            public string ThumbnailImage { get; }

            public int Width { get; }

            public int Height { get; }

            public ImageFileFormat Format { get; }

            #region IsSelected Property
            private bool _isSelected;
            /// <summary>
            /// 是否选中
            /// </summary>
            public bool IsSelected
            {
                get
                {
                    return _isSelected;
                }
                set
                {
                    if (_isSelected != value)
                    {
                        _isSelected = value;
                        OnPropertyChanged(nameof(IsSelected));
                    }
                }
            }
            #endregion IsSelected Property

            #region INotifyPropertyChanged
            [NonSerialized]
            private PropertyChangedEventHandler _propertyChanged;
            public event PropertyChangedEventHandler PropertyChanged
            {
                add
                {
                    _propertyChanged += value;
                }
                remove
                {
                    _propertyChanged -= value;
                }
            }

            protected virtual void OnPropertyChanged(string propertyName)
            {
                _propertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            #endregion INotifyPropertyChanged

            #region Index Property
            private int _index;
            /// <summary>
            /// 选中序号
            /// </summary>
            public int Index
            {
                get
                {
                    return _index;
                }
                set
                {
                    if (_index != value)
                    {
                        _index = value;
                        OnPropertyChanged(nameof(Index));
                    }
                }
            }

            #endregion Index Property
        }
    }
}
