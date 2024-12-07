namespace QTool.Controls
{
    public class QFilterEventArgs
    {
        public object Item { get; }

        public bool Accepted { get; set; }

        public QFilterEventArgs(object item)
        {
            Item = item;
            Accepted = true;
        }
    }
}