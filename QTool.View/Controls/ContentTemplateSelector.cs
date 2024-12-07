using QTool.Controls;
using QTool.View.Contents;
using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace QTool.View.Controls
{
    public class ContentTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var content = item as IBaseContent;
            if (content != null)
            {
                var type = Type.GetType($"QTool.View.Controls.{content.Module}Control");
                if (type != null)
                {
                    FrameworkElementFactory factory = new FrameworkElementFactory(type);
                    factory.SetBinding(ContentControl.ContentProperty, new Binding());
                    return new DataTemplate()
                    {
                        VisualTree = factory
                    };
                }
            }
            return base.SelectTemplate(item, container);
        }
    }
}
