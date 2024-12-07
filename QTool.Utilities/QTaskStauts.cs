using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTool
{

    public enum QTaskStauts
    {
        [Display("待执行")]
        WaitingToRun,

        [Display("执行中")]
        Running,
        
        [Display("执行成功")]
        RanToCompletion,
        
        [Display("执行失败")]
        Faulted
    }
}
