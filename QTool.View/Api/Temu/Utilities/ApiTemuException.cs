using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTool.Api.Temu
{
    public class ApiTemuException : ApiException
    {
        public ApiTemuException(int errorCode, string errorMsg)
            : base(errorMsg)
        {
            ErrorCode = errorCode;
        }

        public int ErrorCode { get; }
    }
}
