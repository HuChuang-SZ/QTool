using CefSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTool.View.Controls
{
    public static class BrowserExt
    {

        /// <summary>
        /// 获取AE平台AppKey
        /// </summary>
        /// <param name="browser"></param>
        /// <returns>访问AppKey，为“0”时表示获取失败</returns>
        public static async Task<string> GetAppKey(this IBrowser browser)
        {
            var script = @"(function () { return window.mtopConfig.appKey; })();";
            var result = await browser.EvaluateScriptAsync(script);
            if (result.Success)
            {
                return result.Result as string;
            }
            return null;
        }
    }
}
