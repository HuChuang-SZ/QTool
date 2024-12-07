using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTool.Controls.Models
{
    public class SortFieldInfo
    {
        public SortFieldInfo(string title, string fields)
        {
            Title = title;
            Fields = fields;
        }

        public string Title { get; }

        public string Fields { get; }
    }
}
