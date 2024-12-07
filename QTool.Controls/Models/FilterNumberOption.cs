using NPOI.OpenXml4Net.OPC;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace QTool.Controls.Models
{


    public class FilterNumberOption : FilterOption
    {
        public FilterNumberOption(QGridViewColumn column, FilterNumberValue value)
            : base(column, value)
        {
            Value = value;
        }

        public new FilterNumberValue Value { get; }

        public override bool Match(object value)
        {
            return Value.ValueCompareTo(value);
        }
    }
}
