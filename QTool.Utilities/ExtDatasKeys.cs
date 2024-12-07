namespace QTool
{
    public enum ExtDatasKeys : byte
    {
        Token = 0,

        /// <summary>
        /// AE
        /// </summary>
        [Display("AE")]
        ChannelId = 1,

        /// <summary>
        /// 西班牙
        /// </summary>
        [Display("西班牙")]
        ChannelId_ES = 2,

        /// <summary>
        /// 全托管
        /// </summary>
        [Display("全托管")]
        ChannelId_Choice = 3,

        /// <summary>
        /// 版本号
        /// </summary>
        ChannelId_Version = 4,
    }
}
