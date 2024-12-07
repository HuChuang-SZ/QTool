using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QTool
{
    public static class GlobalHelper
    {
        private const string DataFolderName = "Datas";

        static GlobalHelper()
        {
            RootPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            AsyncHelper.Exec(ClearHistoryFiles);
        }

        private static void ClearHistoryFiles()
        {
            FileHelper.ClearHistoryFiles(Path.Combine(RootPath, "_temp"), 14);
            FileHelper.ClearHistoryFiles(Path.Combine(RootPath, "Temp"), 14);
            FileHelper.ClearHistoryFiles(Path.Combine(RootPath, "Cache"), 14);
            FileHelper.ClearHistoryFiles(Path.Combine(RootPath, "Logs"), 14);
            FileHelper.ClearHistoryFiles(Path.Combine(RootPath, "Upgrade"), 14);
        }

        public static string GetAccountPath(int accountId)
        {
            return CreatePath(DataFolderName, accountId.ToFolderName());
        }

        public static string GetShopPath(int accountId, int shopId)
        {
            return CreatePath(DataFolderName, accountId.ToFolderName(), shopId.ToFolderName());
        }

        /// <summary>
        /// 请勿修改
        /// </summary>
        private const int _startVal = 658658;

        public static string ToFolderName(this int id)
        {
            return (_startVal + id).ToString("X");
        }

        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <param name="folders">路径"/"</param>
        /// 
        /// <returns></returns>
        public static string CreatePath(params string[] folders)
        {
            if (folders == null || folders.Length == 0)
                throw new ArgumentNullException(nameof(folders));

            var list = new List<string>
            {
                RootPath
            };
            list.AddRange(folders);

            string path = Path.Combine(list.ToArray());

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }



        public static string CreateTempPath(params string[] folders)
        {
            if (folders?.Length > 0)
            {
                var paths = new List<string>
                {
                    TempFolder
                };
                paths.AddRange(folders);

                return CreatePath(paths.ToArray());
            }
            else
            {
                return TempPath;
            }
        }

        /// <summary>
        /// 根目录
        /// </summary>
        public static string RootPath { get; }


        private static string _tempPath;
        private const string TempFolder = "_temp";
        /// <summary>
        /// 临时目录
        /// </summary>
        public static string TempPath
        {
            get
            {

                if (_tempPath == null)
                {
                    _tempPath = CreatePath(TempFolder);
                }
                return _tempPath;
            }
        }

        private static string _dataPath;
        /// <summary>
        /// 数据目录
        /// </summary>
        public static string DataPath
        {
            get
            {
                if (_dataPath == null)
                {
                    _dataPath = CreatePath(DataFolderName);
                }
                return _dataPath;
            }
        }


        /// <summary>
        /// 生成MD5签名
        /// </summary>
        /// <param name="inputVal"></param>
        /// <returns></returns>
        public static string GetMD5(string inputVal)
        {
            var buffer = Encoding.UTF8.GetBytes(inputVal);
            return GetMD5(buffer);
        }

        public static string GetMD5(byte[] buffer)
        {
            using (var md5 = MD5.Create())
            {
                return string.Concat(md5.ComputeHash(buffer).Select(c => c.ToString("x2")));
            }
        }




        /// <summary>
        /// 获取时间戳(毫秒)
        /// </summary>
        /// <returns></returns>
        public static long ToTimestamp()
        {
            return DateTime.Now.ToTimestamp();
        }

        public static string JoinStr(this IEnumerable<string> items, Func<string, bool> predicate, string separator)
        {
            var datas = items.Where(predicate).ToArray();
            if (datas.Length > 0)
            {
                return string.Join(separator, datas);
            }
            else
            {
                return string.Empty;
            }
        }

        public static string JoinStr(this IEnumerable<string> items)
        {
            return items.JoinStr(e => !string.IsNullOrEmpty(e), Environment.NewLine);
        }

        /// <summary>
        /// 获取错误消息
        /// </summary>
        /// <param name="data"></param>
        /// <param name="columnNames"></param>
        /// <returns></returns>
        public static string GetErrors(this IDataErrorInfo data, params string[] columnNames)
        {
            return JoinStr(columnNames.Select(name => data[name]));
        }

        public static TEnum[] GetEnums<TEnum>()
            where TEnum : struct, Enum
        {
            return Enum.GetValues(typeof(TEnum)).Cast<TEnum>().ToArray();
        }

        public static string ToDisplayName(this Enum enumValue)
        {
            if (enumValue.GetCustomAttributes(out DisplayAttribute[] displayAttrs))
                return displayAttrs[0].Name;
            return enumValue.ToString();
        }

        public static string ToDisplayDescription(this Enum enumValue)
        {
            if (enumValue.GetCustomAttributes(out DisplayAttribute[] displayAttrs))
                return displayAttrs[0].Description;
            return enumValue.ToString();
        }

        public static bool GetCustomAttributes<T>(this Enum enumValue, out T[] customAttributes)
            where T : Attribute
        {
            FieldInfo fi = enumValue.GetType().GetField(enumValue.ToString());
            if (fi != null)
            {
                customAttributes = (T[])fi.GetCustomAttributes(typeof(T), false);
                return customAttributes?.Length > 0;
            }
            customAttributes = null;
            return false;
        }

        public static bool TryGetPageUri(this Enum enumValue, out string uriString)
        {
            FieldInfo fi = enumValue.GetType().GetField(enumValue.ToString());
            if (fi != null)
            {
                var attributes = (PageUriAttribute[])fi.GetCustomAttributes(typeof(PageUriAttribute), false);
                if (attributes.Length > 0 && !String.IsNullOrEmpty(attributes[0].Uri))
                {
                    uriString = attributes[0].Uri;
                    return true;
                }
            }
            uriString = null;
            return false;
        }

        public static string Sign(int accountId, string accountPwd, long ticks, string method, string requestData)
        {
            return GetMD5(string.Join("&", accountId, requestData, accountPwd.ToLower(), method.ToLower(), ticks));
        }

        public static string ToStr(this byte[] bytes)
        {
            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes));

            return string.Concat(bytes.Select(b => b.ToString("x2")));
        }

        public static string CreateTempFile()
        {
            string file = Path.Combine(TempPath, Guid.NewGuid().ToString("N") + ".tmp");
            if (File.Exists(file))
            {
                return CreateTempFile();
            }
            else
            {
                return file;
            }
        }

        public static Exception GetInnerException(this Exception ex)
        {
            Exception exception = ex;
            while (exception.InnerException != null)
            {
                exception = exception.InnerException;
            }

            return exception;
        }

        public static IEnumerable<long> FindByInt64(this string source, string pattern = "\\d+")
        {
            if (!string.IsNullOrEmpty(source))
            {
                var matchCollection = Regex.Matches(source, pattern);
                foreach (Match match in matchCollection)
                {
                    if (long.TryParse(match.Value, out long val))
                    {
                        yield return val;
                    }
                }
            }
        }

        public static int GetPageCount(this int totalCount, int pageSize)
        {
            if (pageSize > 0)
            {
                return (totalCount + (pageSize - 1)) / pageSize;
            }
            else
            {
                return 0;
            }
        }

        public static float CmToPixel(this float cmValue)
        {
            return cmValue * 72 / 2.54f;
        }

        public static TValue GetValue<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue defaultValue)
        {
            if (dict != null && dict.TryGetValue(key, out TValue value))
            {
                return value;
            }
            else
            {
                return defaultValue;
            }
        }
    }
}
