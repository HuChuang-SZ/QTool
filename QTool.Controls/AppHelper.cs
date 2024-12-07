using System.Reflection;
using System.Windows.Interop;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using System;
using Microsoft.Win32;

namespace QTool.Controls
{
    public static class AppHelper
    {
        static AppHelper()
        {
            IsExit = false;

            if (Application.Current != null)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Application.Current.Exit += Current_Exit;
                });
            }
        }

        private static void Current_Exit(object sender, ExitEventArgs e)
        {
            IsExit = true;
        }

        private static ImageSource _icon;
        public static ImageSource Icon
        {
            get
            {
                if (_icon == null)
                {
                    using (var icon = System.Drawing.Icon.ExtractAssociatedIcon(Assembly.GetEntryAssembly().ManifestModule.Name))
                    {
                        if (icon != null)
                            _icon = Imaging.CreateBitmapSourceFromHIcon(icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                    }
                }

                return _icon;
            }
        }

        public static bool IsExit { get; private set; }

        public static bool IsMouse(this FrameworkElement elem)
        {
            if (elem != null && elem.IsInitialized)
            {
                var point = Mouse.GetPosition(elem);
                var rect = new Rect(0, 0, elem.ActualWidth, elem.ActualHeight);
                return rect.Contains(point);
            }
            else
            {
                return false;
            }
        }

        public static Window GetActiveWindow()
        {
            if (Application.Current.CheckAccess())
            {
                QWindow active = null, main = null, last = null;
                foreach (Window item in Application.Current.Windows)
                {
                    if (item.IsVisible && item is QWindow win)
                    {
                        if (win.IsActive)
                            active = win;

                        if (win.IsMain)
                            main = win;

                        if (last == null || last.CreateAt < win.CreateAt)
                            last = win;
                    }
                }

                return active ?? last ?? main;
            }
            else
            {
                return Application.Current.Dispatcher.Invoke(new Func<Window>(GetActiveWindow));
            }
        }

        public static bool TryGetFileName(string filter, out string fileName)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = filter;
            if (dialog.ShowDialog() == true)
            {
                fileName = dialog.FileName;
                return true;
            }
            fileName = null;
            return false;
        }

        public static bool TryOpenFileName(string filter, out string fileName)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = filter;
            if (dialog.ShowDialog() == true)
            {
                fileName = dialog.FileName;
                return true;
            }
            fileName = null;
            return false;
        }

        public static void Resize(this Window window)
        {
            if (window.Owner != null)
            {
                window.Width = window.Owner.ActualWidth * 0.8;
                window.Height = window.Owner.ActualHeight * 0.8;
            }
        }

        public static void SetProgressByContext(this ExecuteSchedule schedule, IExecuteScheduleItem item)
        {
            if (Application.Current == null)
                throw new NotSupportedException();

            if (Application.Current.CheckAccess())
            {
                schedule.SetProgress(item);
            }
            else
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    schedule.SetProgress(item);
                });
            }
        }
    }
}
