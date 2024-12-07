using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTool
{
    public class PageUriAttribute : Attribute
    {
        public PageUriAttribute(string uri)
        {
            Uri = uri;
        }

        public string Uri { get; }
    }
}
