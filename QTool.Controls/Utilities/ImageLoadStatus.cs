using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QTool.Controls.Utilities
{
    public enum ImageLoadStatus : byte
    {
        /// <summary>
        /// 待处理
        /// </summary>
        Pending = 0,

        /// <summary>
        /// 已完成
        /// </summary>
        Completed = 1,

        /// <summary>
        /// 失败
        /// </summary>
        Failed = 2,
    }
}
