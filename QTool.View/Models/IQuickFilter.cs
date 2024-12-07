namespace QTool.View.Models
{
    public interface IQuickFilter<TData>
    {
        int Count { get; set; }
        string Name { get; }

        bool Filter(TData item);
    }
}