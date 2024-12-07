using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTool.Controls.Models
{
    public enum ActivityItemCategory
    {
        /// <summary>
        /// 全部商品
        /// </summary>
        [Display("全部商品")]
        All,

        /// <summary>
        /// 已报名商品
        /// </summary>
        [Display("已报名")]
        Joined,

        /// <summary>
        /// 待提交商品
        /// </summary>
        [Display("待提交")]
        Pending,
    }
}
