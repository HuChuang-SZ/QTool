using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace QTool
{
    public class QWebClient : WebClient
    {

        public QWebClient(QCookieManager manager)
        {
            CookieManager = manager;
            AllowAutoRedirect = true;
            AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
        }

        public QCookieManager CookieManager { get; }

        public bool AllowAutoRedirect { get; set; }

        /// <summary>
        /// 超时时间(毫秒)，默认15秒
        /// </summary>
        public int Timeout { get; set; } = 15000;

        public DecompressionMethods AutomaticDecompression { get; set; }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = base.GetWebRequest(address) as HttpWebRequest;
            request.AllowAutoRedirect = AllowAutoRedirect;
            request.AutomaticDecompression = AutomaticDecompression;
            request.Timeout = Timeout;
            if (request != null)
            {
                request.Headers[HttpRequestHeader.Cookie] = CookieManager.GetCookiesHeader(address);
            }
            return request;
        }


        protected override WebResponse GetWebResponse(WebRequest request, IAsyncResult result)
        {
            WebResponse response = base.GetWebResponse(request, result);
            SetCookies(response);
            return response;
        }


        protected override WebResponse GetWebResponse(WebRequest request)
        {
            WebResponse response = base.GetWebResponse(request);
            SetCookies(response);
            return response;
        }

        private void SetCookies(WebResponse res)
        {
            var response = res as HttpWebResponse;
            if (response != null)
            {
                var strCookies = response.Headers[HttpResponseHeader.SetCookie];
                if (!string.IsNullOrEmpty(strCookies))
                {
                    var container = new CookieContainer();
                    container.SetCookies(res.ResponseUri, strCookies);
                    var cookieCollection = container.GetCookies(res.ResponseUri);
                    CookieManager.AddCookies(cookieCollection.Convert());
                }
            }
        }
    }
}
