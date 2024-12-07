using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.IO;
using QTool.Controls.Utilities;


namespace QTool.Controls
{
    public class QImage : Control
    {
        static QImage()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(QImage), new FrameworkPropertyMetadata(typeof(QImage)));
        }

        public QImage()
        {
            IsVisibleChanged += OnIsVisibleChanged;
        }

        public string UriSource
        {
            get { return (string)GetValue(UriSourceProperty); }
            set { SetValue(UriSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UriSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UriSourceProperty =
            DependencyProperty.Register("UriSource", typeof(string), typeof(QImage), new UIPropertyMetadata(null, OnUriSourceChanged));


        private static void OnUriSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((QImage)d).OnLoadImage();
        }

        private ImageLoadThumbItem _currentLoadItem;
        /// <summary>
        /// 下载图片
        /// </summary>
        /// <param name="newValue"></param>
        private void OnLoadImage()
        {
            if (CanBind())
            {
                string uriSource = UriSource;

                if (!string.IsNullOrEmpty(uriSource))
                {
                    int thumbnailValue = GetThumbnailValue();
                    if (_currentLoadItem == null || !_currentLoadItem.IsMatch(uriSource, thumbnailValue))
                    {
                        SetLoadStaus(ImageLoadStatus.Pending);
                        _currentLoadItem = new ImageLoadThumbItem(uriSource, thumbnailValue,  OnLoadImageCallbcak);
                        ImageLoader.Enqueue(_currentLoadItem);
                    }
                }
                else
                {
                    _currentLoadItem = null;
                    SetLoadStaus(ImageLoadStatus.Completed);
                }
            }
        }

        private int GetThumbnailValue()
        {
            var value = Math.Max(ActualWidth, ActualHeight);

            if (!IsThumbnail || value <= 0)
                return 0;
            if (value <= 100)
                return 100;
            else if (value <= 300)
                return 300;
            else
                return ThumbnailMaxValue;
        }


        /// <summary>
        /// 最大缩略图尺寸
        /// </summary>
        public int ThumbnailMaxValue
        {
            get { return (int)GetValue(ThumbnailMaxValueProperty); }
            set { SetValue(ThumbnailMaxValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaxThumbnailValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ThumbnailMaxValueProperty =
            DependencyProperty.Register("ThumbnailMaxValue", typeof(int), typeof(QImage), new PropertyMetadata(600, OnIsThumbnailMaxValueChanged));

        private static void OnIsThumbnailMaxValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((QImage)d).OnLoadImage();
        }

        private bool CanBind()
        {
            return IsInitialized && ActualHeight > 0 && ActualWidth > 0 && IsVisible;
        }

        private void SetLoadCompleted(BitmapImage source, ImageFileFormat format, int width, int height)
        {
            SetLoadStaus(ImageLoadStatus.Completed, source, format, width, height);
        }

        private void SetLoadFailed(string imageUri, string errorMsg)
        {
            SetLoadStaus(ImageLoadStatus.Failed, errorMsg: $"图片加载失败，{errorMsg}\r\n{imageUri}");
        }

        private void SetLoadStaus(ImageLoadStatus loadStatus, ImageSource source = null, ImageFileFormat format = ImageFileFormat.Unkown, int imageWeight = 0, int imageHeight = 0, string errorMsg = null)
        {
            SetCurrentValue(LoadStatusProperty, loadStatus);
            SetCurrentValue(SourceProperty, source);
            SetCurrentValue(ErrorMsgProperty, errorMsg);
            SetCurrentValue(ImageFileFormatProperty, format);
            SetCurrentValue(ImageWidthProperty, imageWeight);
            SetCurrentValue(ImageHeightProperty, imageHeight);
        }

        /// <summary>
        /// 下载完后绑定图片，UI显示
        /// </summary>
        /// <param name="item"></param>
        private void OnLoadImageCallbcak(ImageLoadThumbItem item)
        {
            if (CheckAccess())
            {
                if (item == _currentLoadItem)
                {
                    if (item.Status == ImageLoadStatus.Completed)
                    {
                        try
                        {
                            if (CanBind())
                            {
                                SetStretchInternal();

                                BitmapImage bitmap = new BitmapImage();

                                using (var stream = File.OpenRead(item.ThumbnailImage))
                                {
                                    bitmap.BeginInit();
                                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                                    bitmap.StreamSource = stream;
                                    bitmap.EndInit();
                                }

                                SetLoadCompleted(bitmap, item.Format, item.Width, item.Height);
                            }
                            else
                            {
                                SetLoadCompleted(null, item.Format, item.Width, item.Height);
                            }
                        }
                        catch (Exception ex)
                        {
                            SetLoadFailed(item.UriString, ex.Message);
                        }
                    }
                    else
                    {
                        SetLoadFailed(item.UriString, item.ErrorMsg);
                    }
                }
            }
            else
            {
                Dispatcher.BeginInvoke(new Action<ImageLoadThumbItem>(OnLoadImageCallbcak), item);
            }
        }

        #region ErrorMsg
        public string ErrorMsg
        {
            get { return (string)GetValue(ErrorMsgProperty); }
            set { SetValue(ErrorMsgProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ErrorMessage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ErrorMsgProperty =
            DependencyProperty.Register("ErrorMsg", typeof(string), typeof(QImage), new UIPropertyMetadata(null));
        #endregion ErrorMsg


        public ImageLoadStatus LoadStatus
        {
            get { return (ImageLoadStatus)GetValue(LoadStatusProperty); }
            private set { SetValue(LoadStatusProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Status.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LoadStatusProperty =
            DependencyProperty.Register("LoadStatus", typeof(ImageLoadStatus), typeof(QImage), new PropertyMetadata(ImageLoadStatus.Pending));


        #region Source

        public ImageSource Source
        {
            get { return (ImageSource)GetValue(SourceProperty); }
        }

        // Using a DependencyProperty as the backing store for Source.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(ImageSource), typeof(QImage), new UIPropertyMetadata(null));
        #endregion Source


        public Stretch Stretch
        {
            get { return (Stretch)GetValue(StretchProperty); }
            set { SetValue(StretchProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Stretch.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StretchProperty =
            DependencyProperty.Register("Stretch", typeof(Stretch), typeof(QImage), new PropertyMetadata(Stretch.None, OnStretchChanged));

        private static void OnStretchChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((QImage)d).SetStretchInternal();
        }

        private void SetStretchInternal()
        {
            if (CanBind())
            {
                var stretch = Stretch;
                if (stretch == Stretch.None && _currentLoadItem?.Status == ImageLoadStatus.Completed)
                {
                    if (_currentLoadItem.ThumbnailValue != 0 && _currentLoadItem.ThumbnailValue < ActualWidth && _currentLoadItem.ThumbnailValue < ActualHeight
                     || _currentLoadItem.Width < ActualWidth && _currentLoadItem.Height < ActualHeight)
                    {
                        stretch = Stretch.None;
                    }
                    else
                    {
                        stretch = Stretch.Uniform;
                    }
                }
                SetCurrentValue(StretchInternalProperty, stretch);
            }
        }

        public Stretch StretchInternal
        {
            get { return (Stretch)GetValue(StretchInternalProperty); }
        }

        // Using a DependencyProperty as the backing store for SelfStretch.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StretchInternalProperty =
            DependencyProperty.Register("StretchInternal", typeof(Stretch), typeof(QImage), new PropertyMetadata(Stretch.Uniform));




        public ImageFileFormat ImageFileFormat
        {
            get { return (ImageFileFormat)GetValue(ImageFileFormatProperty); }
        }

        // Using a DependencyProperty as the backing store for ImgType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageFileFormatProperty =
            DependencyProperty.Register("ImageFileFormat", typeof(ImageFileFormat), typeof(QImage), new PropertyMetadata(ImageFileFormat.Unkown));




        public int ImageHeight
        {
            get { return (int)GetValue(ImageHeightProperty); }
        }

        // Using a DependencyProperty as the backing store for ImageHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageHeightProperty =
            DependencyProperty.Register("ImageHeight", typeof(int), typeof(QImage), new PropertyMetadata(0));




        public int ImageWidth
        {
            get { return (int)GetValue(ImageWidthProperty); }
        }

        // Using a DependencyProperty as the backing store for ImageWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageWidthProperty =
            DependencyProperty.Register("ImageWidth", typeof(int), typeof(QImage), new PropertyMetadata(0));



        public bool IsThumbnail
        {
            get { return (bool)GetValue(IsThumbnailProperty); }
            set { SetValue(IsThumbnailProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsThumbnail.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsThumbnailProperty =
            DependencyProperty.Register("IsThumbnail", typeof(bool), typeof(QImage), new PropertyMetadata(true, OnIsThumbnailChanged));

        private static void OnIsThumbnailChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
                ((QImage)d).OnIsThumbnailChanged((bool)e.NewValue);
        }


        private void OnIsThumbnailChanged(bool newValue)
        {
            OnLoadImage();
        }

        protected override void OnInitialized(EventArgs e)
        {
            OnLoadImage();
            base.OnInitialized(e);
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            if (IsThumbnail || LoadStatus == ImageLoadStatus.Pending)
            {
                OnLoadImage();
            }
            SetStretchInternal();
        }

        protected virtual void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (IsVisible)
                OnLoadImage();
        }
    }
}
