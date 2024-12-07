using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;

namespace QTool.View.DAL
{
    public class AccountDAL : DbDAL
    {
        public static AccountDAL Current { get; } = new AccountDAL();

        public AccountDAL() : base(GetDbFile(QContext.Current.AccountId, "Account.db"))
        {
        }

        public uint GetHelpTip(string id)
        {
            var record = FindById<HelpTipRecord>(id);

            if (record != null)
            {
                return record.Version;
            }

            return 0;
        }

        public bool SetHelpTip(string id, uint version)
        {
            var data = new HelpTipRecord()
            {
                Id = id,
                Version = version,
                OpenTime = DateTime.Now,
            };

            return Upsert(data);
        }

        public bool SetParameters(AccountParameters model)
        {
            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            return Upsert(model);
        }

        public AccountParameters GetParameters(int accountId)
        {
            return FindById<AccountParameters>(accountId);
        }



        public bool SetShopLoginUser(ShopLoginUser loginUser)
        {
            return Upsert(loginUser);
        }

        public bool TryGetShopLoginUser(int shopId, out ShopLoginUser loginUser)
        {
            if (TryGetShopUserId(shopId, out string userId))
            {
                return TryGetShopLoginUser(userId, out loginUser);
            }
            else
            {
                loginUser = default;
                return false;
            }
        }

        public bool TryGetShopLoginUser(string userId, out ShopLoginUser loginUser)
        {
            loginUser = FindById<ShopLoginUser>(userId);
            return loginUser != null;
        }

        public bool TryGetShopLoginCookie(int shopId, out ShopLoginCookie loginCookie)
        {
            if (TryGetShopUserId(shopId, out string userId))
            {
                return TryGetShopLoginCookie(userId, out loginCookie);
            }
            else
            {
                loginCookie = default;
                return false;
            }
        }

        public bool TryGetShopLoginCookie(string userId, out ShopLoginCookie loginCookie)
        {
            loginCookie = FindById<ShopLoginCookie>(userId);
            return loginCookie != null;
        }

        public bool SetShopLoginCookie(ShopLoginCookie loginCookie)
        {
            return Upsert(loginCookie);
        }

        public bool TryGetShopUserId(int shopId, out string userId)
        {
            lock (this)
            {
                var associated = FindById<ShopAssociated>(shopId);
                if (associated != null)
                {
                    userId = associated.UserId;
                    return true;
                }
                else
                {
                    userId = null;
                    return false;
                }
            }
        }

        public void SetShopUserId(string userId, params int[] shopIds)
        {
            if (shopIds == null || shopIds.Length == 0) throw new ArgumentNullException(nameof(shopIds));
            var col = GetCollection<ShopAssociated>();
            col.DeleteMany(c => c.UserId == userId || shopIds.Contains(c.ShopId));
            col.InsertBulk(shopIds.Select(shopId => new ShopAssociated() { UserId = userId, ShopId = shopId }));
        }

        public bool ShopClearUp(LoginShop shop, out string userId)
        {
            if (TryGetShopUserId(shop.ShopId, out userId))
            {
                Delete<ShopLoginUser>(userId);
                Delete<ShopLoginCookie>(userId);
                DeleteAssociated(userId);
                return true;
            }
            else
            {
                return false;
            }
        }

        private void DeleteAssociated(string userId)
        {
            var associatedCol = GetCollection<ShopAssociated>();
            associatedCol.DeleteMany(u => u.UserId == userId);
        }
    }
}
