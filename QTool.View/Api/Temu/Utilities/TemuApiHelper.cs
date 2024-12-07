using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.IO;
using System.Collections.Concurrent;
using System.Drawing;
using QTool;

namespace QTool.Api.Temu
{
    public static class TemuApiHelper
    {
        private const string _requestUri = "https://seller.kuajingmaihuo.com";

        private static QWebClient CreateClient(this IShopLogin shop, bool isCommon, bool isAntiContent)
        {
            var client = new QWebClient(shop.CookieManager);
            client.Headers[HttpRequestHeader.AcceptEncoding] = QRequestHeader.AcceptEncoding;
            client.Headers[HttpRequestHeader.AcceptLanguage] = QRequestHeader.AcceptLanguage;
            client.Headers[HttpRequestHeader.UserAgent] = QRequestHeader.UserAgent;
            client.Headers["Content-Type"] = "application/json";
            client.Headers["Origin"] = _requestUri;
            client.Headers["sec-ch-ua"] = QRequestHeader.sec_ch_ua;
            client.Headers["sec-ch-ua-mobile"] = "?0";
            client.Headers["sec-ch-ua-platform"] = "\"Windows\"";
            client.Headers["Sec-Fetch-Dest"] = "empty";
            client.Headers["Sec-Fetch-Mode"] = "cors";
            client.Headers["Sec-Fetch-Site"] = "same-origin";
            if (!isCommon)
            {
                client.Headers["mallid"] = shop.Shop.ShopIdentity;
                if (isAntiContent)
                {
                    client.Headers["Anti-Content"] = GetAntiContent();
                }
            }
            client.Encoding = Encoding.UTF8;
            return client;
        }

        private static string GetAntiContent()
        {
            return "0aqAfajYdjKgygd2597epqOfNB0KfDj-IGuURx6gFhVXdTH_DpwTsPwqZkr8eJnWHvfput0w45MK9CAoPVkF23Ld4RZLXzpPs3IZkUTkIjzyMf_zNNASPJt0QqEnRRi5Rb9at3YRi1dsY8ybQv9M5IXrVkCpk8ZeY3WyEDQniTIgcewYLA0DIIawQj9UJjLkgMswmtNjhuRIviNd_tuQDwmCSAgJqvEK7LZAYIOdUoK1T3HV0duoVlhBH225dPQW6DTtaXLj0eIEvCS9aOEo-2DL9yMmjhKZRgpbDKfqbw6fSaCGfHaSH2ciKnh-WhxpJqLLGFswJqo8v0Ec4iB2tnki_H6zDENK3UEywFQlRIX_q3puBJzvH00vVwGBrPvwiBjWC6M0x5OU0DJYRyuBR9sMTOfPVNfZvsw-hTVQOYfgdFP4raCVmADK5Fica5udwIbXwzK7aMFJv-OzTKHXwauYfgKcjkVY8w1MAB6kWtzxzBLGKh0uXYYG48Ssetbq1fcU40paAwEn1yLw-hbHnBScBGe6kpUxHwHkqYQwwSlQ5w31kwXb5NU4v75iNuGWPpVwkbeY6h8LJ2IJeg_1hv-fLYKf93rO7HMFV9LBdinxIYpcM3-tI85SetIln3bf0R-joQh1skHIvr_S1dowbo_wtImq3xnfRElxlyqDRuqs8tmmU_-SqVb0vLlL_XU4ippa9J0RGU3sob95krdTRne7Ug5";
        }

        private readonly static JsonSerializerSettings _defaultSettings = new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore };
        private static string ToJsonString(this object data)
        {
            return JsonConvert.SerializeObject(data, _defaultSettings);
        }

        private static Dictionary<string, Counter> _userIdMaps = new Dictionary<string, Counter>();
        private static Counter GetCounter(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return new Counter();
            }

            lock (_userIdMaps)
            {
                if (!_userIdMaps.TryGetValue(userId, out Counter counter))
                {
                    counter = new Counter(2);
                    _userIdMaps[userId] = counter;
                }
                return counter;
            }
        }
        public static async Task<JObject> RequestApi(this IShopLogin shop, string api, object data, bool isCommon = false,bool isAntiContent = false)
        {
            if (shop is null)
            {
                throw new ArgumentNullException(nameof(shop));
            }

            if (shop.CookieManager == null)
            {
                throw new NotLoggedException(shop.Shop.DisplayName);
            }

            var counter = GetCounter(shop.CookieManager.UserId);
            counter.Begin();
            try
            {
                var requestUri = _requestUri + api;
                string requestData = data.ToJsonString();
                return await RequestApi(shop, requestUri, requestData, isCommon, isAntiContent);
            }
            finally
            {
                counter.End();
            }
        }

        private static async Task<JObject> RequestApi(IShopLogin shop, string requestUri, string requestData, bool isCommon, bool isAntiContent, int retryCount = 3)
        {
            using (var client = shop.CreateClient(isCommon, isAntiContent))
            {
                var jResult = JsonConvert.DeserializeObject<JObject>(await GetResponseContent(client, requestUri, requestData));
                var errorCode = jResult.Value<int?>("error_code") ?? jResult.Value<int>("errorCode");
                if (errorCode == 1000000)
                {
                    return jResult;
                }
                else
                {
                    switch (errorCode)
                    {
                        case 40001:
                            throw new NotLoggedException();
                        //case 4000004:
                        case 50000:
                            if (retryCount > 0)
                            {
                                await Task.Delay(1000);
                                return await RequestApi(shop, requestUri, requestData, isCommon, isAntiContent, retryCount - 1);
                            }
                            break;
                        default:
                            break;
                    }
                    var errorMsg = jResult.Value<string>("error_msg") ?? jResult.Value<string>("errorMsg");
                    if (string.IsNullOrEmpty(errorMsg))
                    {
                        errorMsg = JsonConvert.SerializeObject(jResult);
                    }
                    throw new ApiTemuException(errorCode, errorMsg);
                }
            }
        }

        private static async Task<string> GetResponseContent(QWebClient client, string requestUri, string requestData)
        {
            try
            {
                string responseContent = await client.UploadStringTaskAsync(requestUri, requestData);
                return responseContent;
            }
            catch (WebException ex)
            {
                if (ex.Response != null)
                {
                    using (var dataStream = ex.Response.GetResponseStream())
                    {
                        using (var reader = new StreamReader(dataStream))
                        {
                            return await reader.ReadToEndAsync();
                        }
                    }
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
