using CefSharp;
using CefSharp.Wpf;
using QTool.Api.Temu;
using QTool.Controls;
using QTool.View.Contents;
using QTool.View.DAL;
using System;
using System.Text;
using System.Threading.Tasks;

namespace QTool.View.Models
{
    public class QWebBrowserByTemu : QWebBrowserBase
    {
        public QWebBrowserByTemu(QBrowserBaseContent owner, string address) : base(owner, address)
        {

        }

        public override string DefaultUri => QCookieManagerByTemu.DefaultUriString;

        protected override async Task CheckLogin(QCookie[] cookies)
        {
            var shopLogin = new NewShopLogin(new QCookieManagerByTemu() { Cookies = cookies });
            var api = new TemuAccountApi();
            var user = await api.GetUser(shopLogin);

            SaveLogin(shopLogin.CookieManager, user.UserId, user.Shops);
        }

        protected override PageKind GetPageKind(Uri uri)
        {
            if (string.Compare(uri.Host, "seller.kuajingmaihuo.com", true) == 0)
            {
                if (string.Compare(uri.LocalPath, "/login", true) == 0)
                {
                    return PageKind.Login;
                }
                return PageKind.Home;
            }
            return PageKind.Unknown;
        }

        protected override async Task InputPassword(ChromiumWebBrowser webBrowser)
        {
            try
            {
                await Task.Delay(IntervalMilliseconds * 5);

                if (webBrowser.IsDisposed)
                    return;

                var frame = webBrowser.GetMainFrame();
                if (frame.IsValid && frame.IsFocused)
                {
                    await Task.Delay(IntervalMilliseconds);
                    string loginId = "usernameId";
                    string loginPassword = "passwordId";

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

                            //输入账号
                            result = await frame.EvaluateScriptAsync($"document.getElementById('{loginId}').focus();");
                            if (!result.Success) return;
                            await Task.Delay(IntervalMilliseconds);
                            if (!string.IsNullOrEmpty(shop.Account))
                            {
                                foreach (var item in shop.Account)
                                {
                                    await Task.Delay(20);
                                    webBrowser.GetBrowserHost().SendKeyEvent((int)258u, item, 0);
                                }
                            }
                            await Task.Delay(IntervalMilliseconds * 2);

                            if (!string.IsNullOrEmpty(shop.Password))
                            {
                                //输入密码
                                var password = new RSAHelper().Decrypt(shop.Password);
                                result = await frame.EvaluateScriptAsync($"document.getElementById('{loginPassword}').focus();");
                                if (!result.Success) return;
                                await Task.Delay(IntervalMilliseconds);
                                foreach (var item in password)
                                {
                                    await Task.Delay(20);
                                    webBrowser.GetBrowserHost().SendKeyEvent((int)258u, item, 0);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex);
                QMessageBox.Show(ex);
            }
        }
    }
}
