using CefSharp;
using CefSharp.Wpf;
using Microsoft.Win32;
using QTool.Controls;
using QTool.Controls.Utilities;
using QTool.View.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using IO = System.IO;

namespace QTool.View
{
    /// <summary>
    /// ImageWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ImageDownloader : QWindow
    {
        private readonly ImageDownloaderViewModel _viewModel;

        public ImageDownloader(string html)
        {
            InitializeComponent();
            DataContext = _viewModel = new ImageDownloaderViewModel(this, html);
        }

        public static void ShowDialog(string html, DependencyObject sender)
        {
            if (Application.Current?.CheckAccess() == false)
            {
                Application.Current.Dispatcher.Invoke(new Action<string, DependencyObject>(ShowDialog), new object[] { html, sender });
            }
            else
            {
                var dialog = new ImageDownloader(html);
                dialog.Owner = GetWindow(sender);
                dialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                dialog.Show();
            }
        }

        private void SelectAll_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if ((e.Parameter as bool?) == false)
                images.UnselectAll();
            else
                images.SelectAll();
        }

        private void Invert_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (images.SelectedItems.Count == 0)
            {
                images.SelectAll();
            }
            else
            {
                foreach (var item in _viewModel.FilterImages)
                {
                    item.IsSelected = !item.IsSelected;
                }
            }
        }

        private void DownloadSelect_Click(object sender, RoutedEventArgs e)
        {
            var selectedItems = images.SelectedItems.Cast<ImageDownloaderViewModel.ImageItem>().ToArray();
            if (selectedItems.Length == 0)
            {
                QMessageBox.Show("请先选择需要下载的图片。", this);
            }
            else
            {
                _viewModel.DownloadImages(selectedItems);
            }
        }

        private void DownloadAll_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.DownloadImages(_viewModel.FilterImages.ToArray());
        }

        private void images_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItems = images.SelectedItems.Cast<ImageDownloaderViewModel.ImageItem>().ToArray();
            int index = 0;
            foreach (var item in selectedItems)
            {
                item.Index = ++index;
            }
        }
    }
}
