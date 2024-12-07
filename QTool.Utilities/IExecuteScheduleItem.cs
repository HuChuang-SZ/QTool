namespace QTool
{
    public interface IExecuteScheduleItem
    {
        string Title { get; }

        string Message { get; set; }

        ExecuteStatus Status { get; set; }
    }
}