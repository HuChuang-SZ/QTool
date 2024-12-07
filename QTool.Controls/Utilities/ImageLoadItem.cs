using iTextSharp.text.pdf;
using System;
using System.Collections.Concurrent;

namespace QTool.Controls.Utilities
{
    public class ImageLoadItem
    {
        public ImageLoadItem(string uriString)
        {
            UriString = uriString;
            try
            {
                Status = ImageLoadStatus.Pending;
                ImageFile = ImageLoader.GetTempFile(uriString);
            }
            catch (Exception ex)
            {
                LoadFailed(ex);
            }
        }

        public ConcurrentBag<ImageLoadItem> RelatedItems { get; } = new ConcurrentBag<ImageLoadItem>();

        public string ImageFile { get; }

        public ImageLoadStatus Status { get; private set; }

        public string ErrorMsg { get; private set; }

        public string UriString { get; }

        public void OnLoad()
        {
            try
            {
                Exception error = null;
                for (int i = 0; i < 3; i++)
                {
                    try
                    {
                        if (i > 0)
                        {
                            ClearHistoryFile();
                        }

                        if (ImageLoader.TryDownloadFile(UriString, ImageFile))
                        {
                            LoadCompleted();
                            OnCallback();
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        error = ex;
                    }
                }

                throw error ?? new Exception($"下载文件异常：{UriString}。");
            }
            catch (Exception ex)
            {
                LoadFailed(ex);
            }
        }

        protected virtual void OnCallback()
        {
            foreach (var item in RelatedItems)
            {
                item.OnLoad();
            }
        }

        protected void LoadFailed(Exception error)
        {
            Status = ImageLoadStatus.Failed;
            ErrorMsg = error.Message;
            OnCallback();
        }

        protected virtual void LoadCompleted()
        {
            Status = ImageLoadStatus.Completed;
        }

        protected virtual void ClearHistoryFile()
        {
            FileHelper.Delete(ImageFile);
        }
    }
}