using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace QTool
{
    public enum QModule : byte
    {
        /// <summary>
        /// 卖家中心
        /// </summary>
        [Display("卖家中心 - AE")]
        AeBrowser = 0,

        /// <summary>
        /// 卖家中心
        /// </summary>
        [Display("卖家中心 - Temu")]
        TemuBrowser = 100,
    }
}
