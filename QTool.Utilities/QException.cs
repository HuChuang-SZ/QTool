using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTool
{
    public class QException : Exception
    {
        public QException(string message, int code = 0)
        : base(message)
        {
            ErrorCode = code;
        }

        public int ErrorCode { get; private set; }
    }
}
