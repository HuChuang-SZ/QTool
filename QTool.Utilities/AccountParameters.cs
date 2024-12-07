using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTool
{
    public class AccountParameters
    {
        public int Id { get; set; }
        public int ModuleId { get; set; }
        public int? ShopId { get; set; }

        public bool IsMaximize { get; set; }

        public double WindowTop { get; set; }

        public double WindowLeft { get; set; }

        public double WindowHeight { get; set; }

        public double WindowWidth { get; set; }
    }
}
