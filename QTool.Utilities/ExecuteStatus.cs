using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTool
{
    public enum ExecuteStatus : byte
    {
        [Display("待查询")]
        Waiting = 0,

        [Display("执行中")]
        Executing = 1,

        [Display("成功")]
        Success = 2,

        [Display("跳过")]
        Skip = 3,

        [Display("失败")]
        Error = 4,
    }
}
