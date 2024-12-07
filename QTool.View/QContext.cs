using QTool.Api.Ae;
using QTool.Controls;
using QTool.View.DAL;
using QTool.View.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTool.View
{
    public class QContext
    {
        public static QContext Current { get; } = new QContext();

        public void StartUnread()
        {
            AsyncHelper.Exec(OnUnread);
        }

        public int AccountId { get; private set; }

        public ReadOnlyCollection<LoginShop> Shops { get; private set; }


        public void LoginSuccess(LoginResult result)
        {
            AccountId = result.AccountId;
            Shops = new ReadOnlyCollection<LoginShop>(result.Shops);
        }

        public LoginShop GetLoginShop(int shopId)
        {
            return Shops.FirstOrDefault(s => s.ShopId == shopId);
        }

        public LoginShop GetLoginShop(QPlatform platform, string shopIdentity)
        {
            return Shops.FirstOrDefault(s => s.Platform == platform && string.Compare(s.ShopIdentity, shopIdentity, true) == 0);
        }

        public ShopLogin GetShopLogin(int shopId, bool ignoreNull = false)
        {
            if (TryGetCookieManager(shopId, out QCookieManager cookieManager))
            {
                var loginShop = GetLoginShop(shopId);
                if (loginShop != null)
                {
                    return new ShopLogin(loginShop, cookieManager);
                }
            }

            if (ignoreNull)
            {
                return null;
            }
            else
            {
                var loginShop = GetLoginShop(shopId);
                if (loginShop != null)
                {
                    throw new NotLoggedException(loginShop.DisplayName);
                }
                else
                {
                    throw new NotLoggedException();
                }
            }
        }


        private readonly Dictionary<string, QCookieManager> _cookieManagerDict = new Dictionary<string, QCookieManager>();
        public bool TryGetCookieManager(string userId, out QCookieManager cookieManager)
        {
            if (string.IsNullOrEmpty(userId))
            {
                cookieManager = null;
                return false;
            }
            lock (_cookieManagerDict)
            {
                if (!_cookieManagerDict.TryGetValue(userId, out cookieManager))
                {
                    if (AccountDAL.Current.TryGetShopLoginCookie(userId, out ShopLoginCookie cookie))
                    {
                        cookieManager = cookie.GetCookieManager();
                        cookieManager.CookiesChanged += CookieManager_CookiesChanged;
                        _cookieManagerDict.Add(userId, cookieManager);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            }
        }

        public bool TryGetCookieManager(int shopId, out QCookieManager cookieManager)
        {
            if (AccountDAL.Current.TryGetShopUserId(shopId, out string userId))
            {
                return TryGetCookieManager(userId, out cookieManager);
            }
            else
            {
                cookieManager = null;
                return false;
            }
        }

        private void CookieManager_CookiesChanged(object sender, CookieChangedEventArgs e)
        {
            if (sender is QCookieManager cookieManager)
            {
                AccountDAL.Current.SetShopLoginCookie(new ShopLoginCookie()
                {
                    UserId = cookieManager.UserId,
                    Cookies = cookieManager.Cookies,
                    Platform = cookieManager.Platform,
                    ExtDatas = cookieManager.ExtDatas
                });
            }
        }


        private void OnUnread()
        {
            const int interval = 1000;
            while (!AppHelper.IsExit)
            {
                if (Shops != null)
                {
                    var tasks = new Task[Shops.Count];
                    for (int i = 0; i < Shops.Count; i++)
                    {
                        tasks[i] = RefreshAeUnread(Shops[i]);
                    }
                    Task.WaitAll(tasks.ToArray());
                }
                Task.Delay(interval).Wait();
            }
        }

        public async Task RefreshAeUnread(LoginShop shop)
        {
            if (shop.Platform == QPlatform.AliExpress && (DateTime.Now - shop.UnreadTime).TotalSeconds > 30)
            {
                try
                {
                    var shopLogin = GetShopLogin(shop.ShopId);
                    shop.Unread = await AeShopApi.UnreadCount(shopLogin);
                    shop.Error = null;
                }
                catch (Exception ex)
                {
                    shop.Error = ex.GetInnerException();
                }
                finally
                {
                    shop.UnreadTime = DateTime.Now;
                }
            }
        }

        public bool ShopClearUp(LoginShop shop)
        {
            if (AccountDAL.Current.ShopClearUp(shop, out string userId) && TryGetCookieManager(userId, out QCookieManager cookieManager))
            {
                cookieManager.CookiesChanged -= CookieManager_CookiesChanged;
                cookieManager.ClearAll();
                MainWindowViewModel.Current.ShopClearUp(shop);
            }
            return true;
        }

        public void ShopClearUp(IShop shop)
        {
            if (shop is LoginShop loginShop)
            {
                ShopClearUp(loginShop);
            }
            else
            {
                loginShop = GetLoginShop(shop.ShopId);
                ShopClearUp(loginShop);
            }
        }
    }
}
