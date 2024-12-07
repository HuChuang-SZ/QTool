using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace QTool.Controls
{
    /// <summary>
    /// 按照步骤 1a 或 1b 操作，然后执行步骤 2 以在 XAML 文件中使用此自定义控件。
    ///
    /// 步骤 1a) 在当前项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:QTool.Controls"
    ///
    ///
    /// 步骤 1b) 在其他项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:QTool.Controls;assembly=QTool.Controls"
    ///
    /// 您还需要添加一个从 XAML 文件所在的项目到此项目的项目引用，
    /// 并重新生成以避免编译错误:
    ///
    ///     在解决方案资源管理器中右击目标项目，然后依次单击
    ///     “添加引用”->“项目”->[浏览查找并选择此项目]
    ///
    ///
    /// 步骤 2)
    /// 继续操作并在 XAML 文件中使用控件。
    ///
    ///     <MyNamespace:SearchTextBox/>
    ///
    /// </summary>
    public class SearchTextBox : TextBox
    {
        static SearchTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SearchTextBox), new FrameworkPropertyMetadata(typeof(SearchTextBox)));
        }

        public SearchTextBox()
        {
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, Paste_Executed));
        }

        private void Paste_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            IDataObject data = Clipboard.GetDataObject();

            if (data.GetDataPresent(DataFormats.Text))
            {
                // 获取剪贴板中的文本，并进行粘贴操作
                string text = (string)data.GetData(DataFormats.Text);
                if (!AcceptsReturn)
                {
                    text = text.Replace('\r', ' ').Replace('\n', ' ');
                }
                ((TextBox)sender).Text = text;
                e.Handled = true;
            }
        }

        public SearchMode SearchMode
        {
            get { return (SearchMode)GetValue(SearchModeProperty); }
            set { SetValue(SearchModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SearchMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SearchModeProperty =
            DependencyProperty.Register("SearchMode", typeof(SearchMode), typeof(SearchTextBox), new PropertyMetadata(SearchMode.Vague));



        public bool SupportSearchMode
        {
            get { return (bool)GetValue(SupportSearchModeProperty); }
            set { SetValue(SupportSearchModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SupportSearchMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SupportSearchModeProperty =
            DependencyProperty.Register("SupportSearchMode", typeof(bool), typeof(SearchTextBox), new PropertyMetadata(true));






        public string Placeholder
        {
            get { return (string)GetValue(PlaceholderProperty); }
            set { SetValue(PlaceholderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Placeholder.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PlaceholderProperty =
            DependencyProperty.Register("Placeholder", typeof(string), typeof(SearchTextBox), new PropertyMetadata(null));


    }
}
