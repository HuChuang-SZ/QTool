using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace QTool.Controls.Models
{
    public interface IFilterOption : IComparable
    {
        int DataCount { get; set; }

        object Value { get; }

        bool Match(object value);

        QGridViewColumn Column { get; }

        bool IsSelected { get; set; }

        event EventHandler<SelectedChangedEventArgs<QGridViewColumn>> SelectedChanged;
    }
}
