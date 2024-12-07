using LiteDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static iTextSharp.text.pdf.PdfDocument;
using static NPOI.HSSF.UserModel.HeaderFooter;

namespace QTool.View.DAL
{

    public abstract class DbDAL
    {
        private const string DbPassword = "08Iv1eKT5nMt";
        private static readonly Dictionary<string, LiteDatabase> _dbMaps = new Dictionary<string, LiteDatabase>();

        private LiteDatabase _db;

        public DbDAL(string dbFile)
        {
            lock (_dbMaps)
            {
                string key = dbFile.ToUpper();
                if (!_dbMaps.TryGetValue(key, out LiteDatabase db))
                {
                    db = new LiteDatabase(new ConnectionString()
                    {
                        Filename = dbFile,
                        Password = DbPassword
                    });
                    _dbMaps.Add(key, db);
                }
                _db = db;
            }
        }

        protected ILiteCollection<TData> GetCollection<TData>()
        {
            return _db.GetCollection<TData>();
        }

        protected IEnumerable<TData> FindAll<TData>()
        {
            var col = GetCollection<TData>();
            return col.FindAll();
        }

        protected int DeleteAll<TData>()
        {
            var col = GetCollection<TData>();
            return col.DeleteAll();
        }

        protected BsonValue Insert<TData>(TData data)
        {
            var col = GetCollection<TData>();
            return col.Insert(data);
        }

        protected bool Update<TData>(TData data)
        {
            var col = GetCollection<TData>();
            return col.Update(data);
        }

        protected bool Upsert<TData>(TData data)
        {
            var col = GetCollection<TData>();
            return col.Upsert(data);
        }

        protected TData FindById<TData>(BsonValue id)
        {
            try
            {
                var col = GetCollection<TData>();
                return col.FindById(id);
            }
            catch (Exception ex)
            {
                QMessageHelper.Show($"读取“{typeof(TData).FullName}”失败，失败原因：{ex.Message}");
                return default;
            }
        }

        protected bool Delete<TData>(BsonValue id)
        {
            ILiteCollection<TData> col = GetCollection<TData>();
            return col.Delete(id);
        }

        protected static string GetDbFile(int accountId, string dbFile)
        {
            if (accountId <= 0)
                throw new ArgumentOutOfRangeException(nameof(accountId));

            return Path.Combine(GlobalHelper.GetAccountPath(accountId), dbFile);
        }
    }
}
