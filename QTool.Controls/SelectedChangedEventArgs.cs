using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace QTool.Controls
{
    public class SelectedChangedEventArgs<TData> : EventArgs
    {
        public TData Data { get; }

        public bool IsSelected { get; }


        public SelectedChangedEventArgs(TData item, bool isSelected)
        {
            Data = item;
            IsSelected = isSelected;
        }
    }
}
