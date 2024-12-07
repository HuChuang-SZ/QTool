using QTool.View.Contents;
using System.Windows;
using System.Windows.Controls;

namespace QTool.View.Controls
{
    public class ContentHeaderTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var content = item as IBaseContent;
            FrameworkElement element = container as FrameworkElement;
            if (content != null && element != null)
            {
                return element.FindResource(content.HeaderTemplate) as DataTemplate;

            }
            return base.SelectTemplate(item, container);
        }
    }
}
