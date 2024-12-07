using QTool.View.Entity.Ae;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Threading.Tasks;

namespace QTool.Api.Ae
{
    public static class AeShopApi
    {
        public const string ChannelId_Version = "0";

        public static void GetChannelId(IShopLogin shop, out string shopName)
        {
            var sites = GetShopSites(shop).Result;
            shopName = sites.FirstOrDefault()?.ShopName;
            shop.CookieManager.SetExtValue(ExtDatasKeys.ChannelId_Version, ChannelId_Version);
            foreach (var site in sites)
            {
                switch (site.ChannelSite)
                {
                    case "GLOBAL":
                        switch (site.ShopModel)
                        {
                            case "POP":
                                shop.CookieManager.SetExtValue(ExtDatasKeys.ChannelId, site.ChannelId);
                                shopName = site.ShopName;
                                break;
                            case "ONE_STOP_SERVICE":
                                shop.CookieManager.SetExtValue(ExtDatasKeys.ChannelId_Choice, site.ChannelId);
                                break;
                            default:
                                break;
                        }
                        break;
                    case "ES":
                        shop.CookieManager.SetExtValue(ExtDatasKeys.ChannelId_ES, site.ChannelId);
                        break;
                    default:
                        break;
                }
            }
        }

        public static async Task<AeShopInfo> GetSellerInfo(IShopLogin shop)
        {
            const string api = "mtop.csp.merchant.account.profile.getselleraccountbasicinfo";

            //{"_timezone":-8,"spm":"5261.newworkbench.0.0.6dea4edfMxvoLy","channelId":"36375"}
            var requestData = new
            {
                _timezone = -8,
                //channelId = AeConfig.ChannelId_36375,
            };

            var result = await shop.RequestApi(api, requestData);
            var data = result?.Value<JObject>("data")?.Value<JObject>("data");
            if (data != null)
            {
                return new AeShopInfo()
                {
                    LoginId = data.Value<string>("loginId"),
                    SellerId = data.Value<long>("sellerId"),
                };
            }
            else
            {
                throw new ApiParseDataException();
            }
        }

        public static async Task<AeShopSite[]> GetShopSites(IShopLogin shop)
        {
            const string api = "mtop.ae.csp.nav.shop.list";

            var requestData = new
            {
                _timezone = -8,
            };

            var result = await shop.RequestApi(api, requestData);
            var shopList = result?.Value<JObject>("data")?.Value<JObject>("data")?.Value<JArray>("shopList");
            if (shopList != null)
            {
                var shopSites = new AeShopSite[shopList.Count];
                for (int i = 0; i < shopList.Count; i++)
                {

                    shopSites[i] = new AeShopSite()
                    {
                        ChannelId = shopList[i].Value<string>("channelId"),
                        ChannelType = shopList[i].Value<string>("channelType"),
                        ChannelSite = shopList[i].Value<string>("channelSite"),
                        ShopName = shopList[i].Value<string>("shopName"),
                        ShopLogoUrl = shopList[i].Value<string>("shopLogoUrl"),
                        ShopModel = shopList[i].Value<string>("shopModel"),
                        ShopStatusId = shopList[i].Value<int>("shopStatusId"),
                        ShopStatusName = shopList[i].Value<string>("shopStatusName"),
                    };
                }
                return shopSites;
            }
            else
            {
                throw new ApiParseDataException();
            }
        }


        /// <summary>
        /// 获取未读消息
        /// </summary>
        /// <param name="shop"></param>
        /// <returns></returns>
        /// <exception cref="ApiParseDataException"></exception>
        public static async Task<int> UnreadCount(IShopLogin shop)
        {
            const string api = "mtop.csp.im.use.web.seller.unreadcount";

            var data = new
            {
                //channelId = AeConfig.ChannelId_36375,
                _timezone = -8
            };
            var jResult = await shop.RequestApi(api, data, AppKeys.Default);
            var jData = jResult.Value<JObject>("data");
            if (jData != null)
            {
                return jData.Value<int>("result");
            }
            else
            {
                throw new ApiParseDataException("未发现“data”节点");
            }
        }
    }
}
