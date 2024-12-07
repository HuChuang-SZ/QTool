using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace QTool.Controls.Models
{
    public class FilterArrayOption : FilterOption
    {
        public FilterArrayOption(QGridViewColumn column, object value) : base(column, value)
        {
        }

        public override bool Match(object value)
        {
            var array = value as IEnumerable;
            int index = 0;
            if (array != null)
            {
                foreach (var val in array)
                {
                    index++;
                    if (base.Match(val))
                        return true;
                }

            }
            if (index == 0)
                return base.Match(null);
            else
                return false;

        }
    }
}
