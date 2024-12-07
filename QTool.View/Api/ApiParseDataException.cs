using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTool.Api
{
    public class ApiParseDataException : Exception
    {
        public ApiParseDataException(string message = "返回数据结构发送改变，请稍后重试", Exception innerException = null)
            : base(message, innerException)
        {

        }

    }
}
