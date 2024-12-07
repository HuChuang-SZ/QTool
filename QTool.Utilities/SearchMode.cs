using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTool
{
    public enum SearchMode
    {
        /// <summary>
        /// 模糊
        /// </summary>
        [Display("模糊")]
        Vague,

        /// <summary>
        /// 精准
        /// </summary>
        [Display("精准")]
        Accurate,

        /// <summary>
        /// 开头
        /// </summary>
        [Display("开头")]
        StartsWith,

        /// <summary>
        /// 结尾
        /// </summary>
        [Display("结尾")]
        EndsWith

    }
}
