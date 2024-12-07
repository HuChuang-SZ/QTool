using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTool
{
    public static class QRequestHeader
    {
        public static string UserAgent { get; } = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/109.0.0.0 Safari/537.36 Edg/109.0.1518.140";

        public static string sec_ch_ua { get; } = "\"Not_A Brand\";v=\"109\", \"Chromium\";v=\"109\", \"Microsoft Edge\";v=\"109\"";

        public static string AcceptLanguage { get; } = "zh-CN,zh;q=0.9,en;q=0.8,en-GB;q=0.7,en-US;q=0.6";

        public static string AcceptEncoding { get; } = "gzip, deflate, br";
    }
}
