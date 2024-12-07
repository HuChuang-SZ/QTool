using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTool.View.Models
{
    public class QModuleGroup
    {
        public string Name { get; }

        public QModuleInfo[] Modules { get; }

        public QModuleGroup(string name, params QModuleInfo[] modules)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException($"“{nameof(name)}”不能为 null 或空白。", nameof(name));
            }

            Name = name;
            Modules = modules ?? throw new ArgumentNullException(nameof(modules));
        }
    }
}
