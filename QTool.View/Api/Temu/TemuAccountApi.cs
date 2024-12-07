using QTool.View.Entity.Temu;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QTool.Api.Temu
{
    public class TemuAccountApi
    {

        public async Task<TemuUser> GetUser(IShopLogin shop)
        {
            const string api = "/bg/quiet/api/mms/userInfo";
            var jResult = await shop.RequestApi(api, new { }, true);

            var user = new TemuUser
            {
                UserId = jResult.FindValue<string>("result.userId"),

                Shops = GetShopList(jResult.Find<JArray>("result.companyList")).ToArray()
            };

            return user;
        }

        private IEnumerable<ShopBindModel> GetShopList(JArray companyList)
        {
            if (companyList != null)
            {
                foreach (var jCompany in companyList)
                {
                    var malInfoList = jCompany.Find<JArray>("malInfoList");

                    if (malInfoList != null)
                    {
                        foreach (var malInfo in malInfoList)
                        {
                            var shopName = malInfo.FindValue<string>("mallName");
                            yield return new ShopBindModel()
                            {
                                Platform = QPlatform.Temu,
                                ShopIdentity = malInfo.FindValue<string>("mallId"),
                                DisplayName = shopName,
                            };
                        }
                    }
                }
            }
        }
    }
}
