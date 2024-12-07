using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace QTool.View.Models
{
    public class HelpTipStep
    {
        public HelpTipStep(string title, string content, string elementName = null)
        {
            Title = title ?? throw new ArgumentNullException(nameof(title));
            Content = content ?? throw new ArgumentNullException(nameof(content));

            ElementName = elementName;
        }

        public string ElementName { get; }

        public string Title
        {
            get;
        }

        public string Content
        {
            get;
        }
    }
}
