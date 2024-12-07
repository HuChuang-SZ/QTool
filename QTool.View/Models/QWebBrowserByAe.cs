using CefSharp.Wpf;
using CefSharp;
using System;
using System.Text;
using System.Threading.Tasks;
using QTool.Controls;
using QTool.View.Contents;
using QTool.Api.Ae;
using QTool.View.DAL;

namespace QTool.View.Models
{
    public class QWebBrowserByAe : QWebBrowserBase
    {
        public override string DefaultUri => QCookieManagerByAe.DefaultUriString;

        public QWebBrowserByAe(QBrowserBaseContent owner, string address) : base(owner, address)
        {
        }

        protected override PageKind GetPageKind(Uri uri)
        {
            switch (uri.Host.ToLower())
            {
                ///成功登录后执行
                case "csp.aliexpress.com":
                    return PageKind.Home;
                case "login.aliexpress.com":
                    return PageKind.Login;
                default:
                    return PageKind.Unknown;
            }
        }

        /// <summary>
        /// 输入密码
        /// </summary>
        /// <param name="webBrowser"></param>
        /// <returns></returns>
        protected async override Task InputPassword(ChromiumWebBrowser webBrowser)
        {
            try
            {
                await Task.Delay(IntervalMilliseconds * 5);

                if (webBrowser.IsDisposed)
                    return;

                var frame = webBrowser.GetMainFrame();
                if (frame.IsValid && frame.IsFocused)
                {
                    string loginId = "fm-login-id";
                    string loginPassword = "fm-login-password";


                    //监控账号密码修改
                    await Task.Delay(100);
                    var jString = new StringBuilder();
                    jString.AppendLine($"var loginId = '{loginId}';");
                    jString.AppendLine($"var loginPwd = '{loginPassword}';");
                    jString.AppendLine("var QAttrName = 'QBind';");
                    jString.AppendLine("function QBind(retryCount) {");
                    jString.AppendLine("    if (retryCount > 0) {");
                    jString.AppendLine("        var elemId = document.getElementById(loginId);");
                    jString.AppendLine("        var elemPwd = document.getElementById(loginPwd);");

                    jString.AppendLine("        if (!elemId.hasAttribute(QAttrName)) {");
                    jString.AppendLine("            elemId.setAttribute(QAttrName, 'Account');");
                    jString.AppendLine("            elemId.addEventListener('change', function () { CefSharp.PostMessage({ 'Account': this.value }); });");
#if DEBUG
                    jString.AppendLine("            CefSharp.PostMessage('Account绑定成功');");
#endif
                    jString.AppendLine("        }");

                    jString.AppendLine("        if (!elemPwd.hasAttribute(QAttrName)) {");
                    jString.AppendLine("            elemPwd.setAttribute(QAttrName, 'Password');");
                    jString.AppendLine("            elemPwd.addEventListener('change', function () { CefSharp.PostMessage({ 'Password': this.value }); });");
#if DEBUG
                    jString.AppendLine("            CefSharp.PostMessage('Password绑定成功');");
#endif
                    jString.AppendLine("        }");

                    jString.AppendLine("        setTimeout(QBind, 10, retryCount - 1);");
                    jString.AppendLine("    }");
                    jString.AppendLine("}");
                    jString.AppendLine("QBind(500);");
                    var result = await frame.EvaluateScriptAsync(jString.ToString());

                    if (!result.Success) return;

                    if (Owner.ShopId > 0)
                    {
                        if (AccountDAL.Current.TryGetShopLoginUser(Owner.ShopId, out ShopLoginUser shop))
                        {
                            await Task.Delay(1000);

                            //移除显示密码按钮
                            result = await frame.EvaluateScriptAsync("Array.from(document.getElementsByClassName('comet-input-suffix')).forEach(function(element,index,array){ element.remove(); });");
                            if (!result.Success) return;

                            if (!string.IsNullOrEmpty(shop.Account))
                            {
                                //输入账号
                                result = await frame.EvaluateScriptAsync($"document.getElementById('{loginId}').focus();");
                                if (!result.Success) return;
                                await Task.Delay(IntervalMilliseconds);
                                foreach (var item in shop.Account)
                                {
                                    webBrowser.GetBrowserHost().SendKeyEvent((int)258u, item, 0);
                                }
                            }

                            //输入密码
                            var password = new RSAHelper().Decrypt(shop.Password);
                            if (!string.IsNullOrEmpty(password))
                            {
                                result = await frame.EvaluateScriptAsync($"document.getElementById('{loginPassword}').type = 'password'; document.getElementById('{loginPassword}').focus();");
                                if (!result.Success) return;
                                await Task.Delay(IntervalMilliseconds);
                                foreach (var item in password)
                                {
                                    webBrowser.GetBrowserHost().SendKeyEvent((int)258u, item, 0);
                                }
                            }
                        }
                        else
                        {
                            var loginShop = QContext.Current.GetLoginShop(Owner.ShopId);
                            if (loginShop != null)
                            {
                                //输入账号
                                result = await frame.EvaluateScriptAsync($"document.getElementById('{loginId}').focus();");
                                if (!result.Success) return;
                                await Task.Delay(IntervalMilliseconds);
                                foreach (var item in loginShop.ShopIdentity)
                                {
                                    webBrowser.GetBrowserHost().SendKeyEvent((int)258u, item, 0);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                QMessageBox.Show(ex);
            }
        }


        protected override async Task CheckLogin(QCookie[] cookies)
        {
            var shopLogin = new NewShopLogin(new QCookieManagerByAe() { Cookies = cookies });
            var seller = await AeShopApi.GetSellerInfo(shopLogin);
            AeShopApi.GetChannelId(shopLogin, out string shopName);
            var shop = new ShopBindModel()
            {
                ShopIdentity = seller.LoginId.ToLower(),
                DisplayName = shopName,
                Platform = QPlatform.AliExpress
            };

            SaveLogin(shopLogin.CookieManager, shop.ShopIdentity, shop);
        }
    }

}
