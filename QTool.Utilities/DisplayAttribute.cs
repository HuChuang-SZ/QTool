using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTool
{
    [AttributeUsage(AttributeTargets.All)]
    public class DisplayAttribute : Attribute
    {
        public DisplayAttribute(string name)
        {
            Name = name;
        }

        public DisplayAttribute(string name, string description)
            : this(name)
        {
            Description = description;
        }

        public string Name { get; }

        public string Description { get; set; }

        public string Group { get; set; }
    }
}
