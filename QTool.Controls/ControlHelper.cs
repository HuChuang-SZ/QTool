using System.Linq;
using System.Reflection;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;

namespace QTool.Controls
{
    public static class ControlHelper
    {
        public static Rect GetRect(this Window win, string name)
        {
            var elem = win.FindName(name) as FrameworkElement;
            if (elem == null)
            {
                if (!win.TryFindChild(name, out elem))
                {
                    throw new QException($"未找到控件名（{name}）");
                }
            }

            var location = elem.PointToScreen(new Point(0, 0));

            return new Rect(location.X - win.Left, location.Y - win.Top, elem.ActualWidth, elem.ActualHeight);
        }

        public static bool TryFindChild<TElement>(this DependencyObject obj, out TElement element) where TElement : FrameworkElement
        {
            return TryFindChild(obj, null, out element);
        }

        public static bool TryFindChild<TElement>(this DependencyObject obj, string name, out TElement element) where TElement : FrameworkElement
        {
            var childrenCount = VisualTreeHelper.GetChildrenCount(obj);
            for (int i = 0; i < childrenCount; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child is TElement elem && (string.IsNullOrEmpty(name) || elem.Name == name))
                {
                    element = elem;
                    return true;
                }
                else
                {
                    if (TryFindChild(child, name, out element))
                        return true;
                }
            }
            element = default;
            return false;
        }

        public static void SetOwner(this Window dialog, Window owner = null)
        {
            if (owner == null)
            {
                var elem = Keyboard.FocusedElement as DependencyObject;
                if (elem != null)
                {
                    owner = Window.GetWindow(elem);
                }
            }

            if (owner == null)
            {
                dialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }
            else
            {
                dialog.Owner = owner;
                dialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }
        }

        public static Window GetWindow()
        {
            var elem = Keyboard.FocusedElement as DependencyObject;
            if (elem != null)
            {
                return Window.GetWindow(elem);
            }

            return null;
        }

        public static T FindParent<T>(this DependencyObject child)
            where T : DependencyObject
        {
            var parent = child;
            while (parent != null)
            {
                parent = GetParent(parent);
                if (parent is T tParent)
                {
                    return tParent;
                }
            }
            return null;
        }

        public static bool TryFindParent<T>(this DependencyObject child, out T tParent)
            where T : DependencyObject
        {
            tParent = child.FindParent<T>();
            return tParent != null;
        }

        private static readonly PropertyInfo InheritanceContextProp = typeof(DependencyObject).GetProperty("InheritanceContext", BindingFlags.NonPublic | BindingFlags.Instance);
        public static DependencyObject GetParent(this DependencyObject child)
        {
            if (child != null)
            {
                var parent = LogicalTreeHelper.GetParent(child);
                if (parent == null)
                {
                    if (child is FrameworkElement)
                    {
                        parent = VisualTreeHelper.GetParent(child);
                    }
                    if (parent == null && child is ContentElement content)
                    {
                        parent = ContentOperations.GetParent(content);
                    }
                    if (parent == null)
                    {
                        parent = InheritanceContextProp.GetValue(child, null) as DependencyObject;
                    }
                }
                return parent;
            }
            return null;
        }

        public static T FindParent<T>(this ExecutedRoutedEventArgs e)
            where T : DependencyObject
        {
            var elem = e.Source as T ?? e.OriginalSource as T;
            if (elem != null) return elem;

            var obj = e.OriginalSource as DependencyObject;
            return obj?.FindParent<T>();

        }

        public static bool TryGetSelectedItems<T>(this QListView listView, out T[] selectedItems)
        {
            if (listView != null)
            {
                if (listView.SelectedItems.Count > 0)
                {
                    selectedItems = listView.SelectedItems.Cast<T>().ToArray();
                    return true;
                }
                else
                {
                    QMessageBox.Show("请先选择需要操作项");
                }
                selectedItems = default;
                return false;
            }
            else
            {
                QMessageBox.Show($"“{nameof(listView)}”不能为空。");
            }
            selectedItems = default;
            return false;
        }

        public static bool TryGetSelectedItems<T>(this ExecutedRoutedEventArgs e, out T[] selectedItems)
        {
            if (e.Parameter is T item)
            {
                selectedItems = new T[] { item };
                return true;
            }
            else
            {
                var listView = e.FindParent<QListView>();
                if (listView != null)
                {
                    return listView.TryGetSelectedItems(out selectedItems);
                }
                else
                {
                    selectedItems = default;
                    return false;
                }
            }
        }

        public static void Invert(this ListBox listBox)
        {
            if (listBox.ItemContainerGenerator != null)
            {
                for (int i = 0; i < listBox.Items.Count; i++)
                {
                    if (listBox.ItemContainerGenerator.ContainerFromIndex(i) is ListBoxItem listBoxItem)
                    {
                        listBoxItem.IsSelected = !listBoxItem.IsSelected;
                    }
                }
            }
        }





        public static T[] GetSelectedItems<T>(this ListBox listBox)
        {
            return listBox.SelectedItems.Cast<T>().ToArray();
        }



        public static void SetSelectedItems<T>(this ListBox listBox, T[] items)
        {
            if (listBox.ItemContainerGenerator != null)
            {
                listBox.UnselectAll();
                if (items != null)
                {
                    foreach (var item in items)
                    {
                        if (listBox.ItemContainerGenerator.ContainerFromItem(item) is ListBoxItem listBoxItem)
                        {
                            listBoxItem.IsSelected = true;
                        }
                    }
                }
            }
        }

        public static bool TryGetListViewItemContent<TData>(this MouseButtonEventArgs e, out TData data)
        {
            if (e.OriginalSource is DependencyObject d && d.TryFindParent(out ListViewItem listViewItem) && listViewItem.Content is TData tData)
            {
                data = tData;
                return true;
            }
            else
            {
                data = default(TData);
                return false;
            }
        }
    }
}
