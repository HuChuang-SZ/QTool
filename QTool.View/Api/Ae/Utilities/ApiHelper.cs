using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace QTool.Api.Ae
{
    public static class ApiHelper
    {
        #region Public Methods
        public static async Task DownloadFile(this IShopLogin shop, string uri, string fileName)
        {
            using (var client = shop.CreateClient())
            {
                await client.DownloadFileTaskAsync(uri, fileName);
            }
        }

        public static async Task<byte[]> DownloadData(this IShopLogin shop, string uri)
        {
            using (var client = shop.CreateClient())
            {
                return await client.DownloadDataTaskAsync(uri);
            }
        }

        public static async Task<string> DownloadString(this IShopLogin shop, string uri)
        {
            return await DownloadString(shop, uri, Encoding.UTF8);
        }

        public static async Task<string> DownloadString(this IShopLogin shop, string uri, Encoding encoding)
        {
            using (var client = shop.CreateClient())
            {
                client.Encoding = encoding;
                return await client.DownloadStringTaskAsync(uri);
            }
        }

        private readonly static ApiErrorCode[] RetryErrorCodes = { ApiErrorCode.SESSION_EXPIRED, ApiErrorCode.TOKEN_EXOIRED };
        public static async Task<JObject> RequestApi(this IShopLogin shop, string api, object data, AppKeys appKey = AppKeys.Default, Action<QWebClient> setHeader = null, Action<IShopLogin, NameValueCollection> setDatas = null, int retryCont = 0)
        {
            using (var client = shop.CreateClient())
            {
                try
                {
                    string requestData = data.ToJsonString();

                    setHeader?.Invoke(client);

                    var datas = GetCommonData(api, AeConfig.GetAppKey(appKey), shop.CookieManager.GetExtValue(ExtDatasKeys.Token), requestData);

                    setDatas?.Invoke(shop, datas);

                    var requestUri = datas.GetRequestUri();

                    return await client.RequestApi(requestUri, requestData);
                }
                catch (ApiAeException ex)
                {
                    if (RetryErrorCodes.Contains(ex.Code))
                    {
                        if (retryCont < 2)
                        {
                            if (retryCont >= 1)
                            {
                                Task.Delay(500).Wait();
                            }
                            return await shop.RequestApi(api, data, appKey, setHeader, setDatas, retryCont + 1);
                        }
                    }
                    throw;
                }
                catch
                {
                    throw;
                }
            }
        }
        #endregion Public Methods

        #region Private Methods
        private static QWebClient CreateClient(this IShopLogin shop)
        {
            var client = new QWebClient(shop.CookieManager);
            client.Headers[HttpRequestHeader.AcceptEncoding] = QRequestHeader.AcceptEncoding;
            client.Headers[HttpRequestHeader.AcceptLanguage] = QRequestHeader.AcceptLanguage;
            client.Headers[HttpRequestHeader.UserAgent] = QRequestHeader.UserAgent;
            client.Headers["Content-Type"] = "application/x-www-form-urlencoded";
            client.Headers["Origin"] = "https://csp.aliexpress.com";
            client.Headers["sec-ch-ua"] = QRequestHeader.sec_ch_ua;
            client.Headers["sec-ch-ua-mobile"] = "?0";
            client.Headers["sec-ch-ua-platform"] = "\"Windows\"";
            client.Headers["Sec-Fetch-Dest"] = "empty";
            client.Headers["Sec-Fetch-Mode"] = "cors";
            client.Headers["Sec-Fetch-Site"] = "same-site";
            client.Encoding = Encoding.UTF8;
            return client;
        }

        private readonly static JsonSerializerSettings _defaultSettings = new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore };
        private static string ToJsonString(this object data)
        {
            return JsonConvert.SerializeObject(data, _defaultSettings);
        }


        private static async Task<JObject> RequestApi(this QWebClient client, string requestUri, string data = null)
        {
            string responseContent = await client.UploadStringTaskAsync(requestUri, GetRequestData(nameof(data), data));

            var jResult = JsonConvert.DeserializeObject<JObject>(responseContent);

            var jRet = jResult.Value<JArray>("ret");
            if (jRet.Count > 0)
            {
                foreach (var jItem in jRet)
                {
                    var ret = jItem.Value<string>();
                    if (!string.IsNullOrEmpty(ret))
                    {
                        string code;
                        var index = ret.IndexOf("::");
                        if (index > 0)
                        {
                            code = ret.Substring(0, index);
                        }
                        else
                        {
                            code = ret;
                        }
                        switch (code)
                        {
                            case "SUCCESS":
                                return jResult;
                            case "FAIL_SYS_TOKEN_EXOIRED":
                                throw new ApiAeException(ApiErrorCode.TOKEN_EXOIRED);
                            case "FAIL_SYS_SESSION_EXPIRED":
                                throw new NotLoggedException();
                            case "FAIL_SYS_ILLEGAL_ACCESS":
                                throw new ApiAeException(ApiErrorCode.ILLEGAL_ACCESS);
                            case "FAIL_SYS_SERVICE_INNER_FAULT":
                                throw new ApiAeException(ApiErrorCode.SERVICE_INNER_FAULT);
                            case "UNKNOWN_FAIL_CODE":
                                throw new ApiAeException(ApiErrorCode.UNKNOWN_FAIL_CODE, jResult.GetError());
                            case "Error":
                                throw new ApiAeException(ApiErrorCode.Error, ret.Substring(index + 2));
                            default:
                                throw new ApiAeException(ret);
                        }
                    }
                }
            }
            throw new ApiAeException(responseContent);
        }

        private static string GetRequestUri(this NameValueCollection datas)
        {
            StringBuilder uri = new StringBuilder();
            uri.Append("https://seller-acs.aliexpress.com/h5/").Append(datas["api"]).Append("/1.0/?");
            if (datas != null)
            {
                bool isFirst = true;
                foreach (var key in datas.AllKeys)
                {
                    if (isFirst)
                    {
                        isFirst = false;
                    }
                    else
                    {
                        uri.Append("&");
                    }
                    uri.Append(GetRequestData(key, datas[key]));
                }
            }
            return uri.ToString();
        }

        private static string GetRequestData(string name, string value)
        {
            return string.Concat(name, "=", WebUtility.UrlEncode(value));
        }

        /// <summary>
        /// 生成签名
        /// </summary>
        /// <param name="data"></param>
        /// <param name="timestemp"></param>
        /// <returns></returns>
        private static string GetSign(string appKey, string appToken, long timestamp, string data)
        {
            return GlobalHelper.GetMD5(string.Join("&", appToken, timestamp, appKey, data));
        }

        private static NameValueCollection GetCommonData(string api, string appKey, string appToken, string data)
        {
            long timestamp = GlobalHelper.ToTimestamp();
            var datas = new NameValueCollection();
            datas["jsv"] = "2.3.16";
            datas["appKey"] = appKey;
            datas["t"] = timestamp.ToString();
            datas["sign"] = GetSign(appKey, appToken, timestamp, data);
            datas["v"] = "1.0";
            datas["url"] = api;
            datas["api"] = api;
            datas["type"] = "originaljson";
            datas["dataType"] = "json";
            datas["headers"] = "[object Object]";
            return datas;
        }
        #endregion Private Methods

        public static string GetError(this JObject result)
        {
            string error = result.FindValue<string>("data.error", true) ?? result.FindValue<string>("data.errorMsg", true);
            if (string.IsNullOrEmpty(error))
            {
                error = JsonConvert.SerializeObject(result);
            }

            return error;
        }
    }
}
