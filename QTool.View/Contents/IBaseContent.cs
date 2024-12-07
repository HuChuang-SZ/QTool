using System.ComponentModel;

namespace QTool.View.Contents
{
    public interface IBaseContent : INotifyPropertyChanged
    {
        bool IsWaiting { get; }

        QModule Module { get; }

        string WaitMsg { get; }

        string HeaderTemplate { get; }
    }
}