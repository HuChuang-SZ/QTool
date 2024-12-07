using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTool.Api.Ae
{
    public class AeConfig
    {
        ///// <summary>
        ///// 渠道Id
        ///// </summary>
        //public const string ChannelId_36375 = "36375";

        //public const string ChannelId_154678 = "154678";

        ///// <summary>
        ///// 平台标识
        ///// </summary>
        //public const string Spm = "5261.15031002.orderTable.6.678b601a9Az6AB";

        private const string AppKeyByDefault = "30267743";

        private const string AppKeyBySession = "25556077";


        internal static string GetAppKey(AppKeys appKey)
        {
            switch (appKey)
            {
                case AppKeys.Session:
                    return AppKeyBySession;
                default:
                    return AppKeyByDefault;
            }
        }
    }
}
