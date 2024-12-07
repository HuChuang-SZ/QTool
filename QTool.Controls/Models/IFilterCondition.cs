using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace QTool.Controls.Models
{
    [TypeConverter(typeof(FilterConditionConverter))]
    public interface IFilterCondition
    {
        void CreateOptions(QGridViewColumn column);

        bool Match(object item);

        ReadOnlyCollection<IFilterOption> Options { get; }

        bool IsSelected { get; }

        bool IsGenerating { get; }

        string Message { get; }
    }
}
