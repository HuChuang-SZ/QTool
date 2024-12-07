using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTool.Controls
{
    public class InvalidImageFormatException : Exception
    {
        public InvalidImageFormatException(string message = "不支持当前图片格式，请联系客服。")
        : base(message)
        { }
    }
}
