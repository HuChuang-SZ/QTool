using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTool.Api
{
    public static class JsonExt
    {
        public static T Find<T>(this JToken jToken, string path, bool ignoreNull = false)
            where T : JToken
        {
            if (jToken is null)
            {
                throw new ArgumentNullException(nameof(jToken));
            }

            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException($"“{nameof(path)}”不能为 null 或空白。", nameof(path));
            }

            var token = jToken.SelectToken(path);

            if (token == null || token.Type == JTokenType.Null)
            {
                if (ignoreNull)
                    return default;
                else
                    throw new ApiParseDataException(jToken.GetInvalidPathMessage(path));
            }
            else if (token is T)
            {
                return (T)token;
            }
            else
            {
                throw new ApiParseDataException($"无法将{GetFullPath(jToken, path)}“{token.GetType().Name}”类型转化为“{typeof(T).Name}”类型。");
            }
        }

        public static T FindValue<T>(this JToken jToken, string path, bool ignoreNull = false)
        {
            if (jToken is null)
            {
                throw new ArgumentNullException(nameof(jToken));
            }

            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException($"“{nameof(path)}”不能为 null 或空白。", nameof(path));
            }

            var token = jToken.SelectToken(path);

            if (token == null)
            {
                if (ignoreNull)
                    return default;
                else
                    throw new ApiParseDataException(jToken.GetInvalidPathMessage(path));

            }
            else
            {
                try
                {
                    return token.Value<T>();
                }
                catch (Exception ex)
                {
                    throw new ApiParseDataException(ex.GetExceptionMessage(jToken, path));
                }
            }
        }

        private static string GetFullPath(JToken jToken, string path)
        {
            string fullPath;
            if (string.IsNullOrEmpty(jToken.Path))
            {
                fullPath = path;
            }
            else
            {
                fullPath = string.Join(".", jToken.Path, path);
            }
            return fullPath;
        }

        private static string GetExceptionMessage(this Exception innerException, JToken jToken, string path)
        {
            string errorMessage = $"解析“{GetFullPath(jToken, path)}”节点时发生错误";
            if (innerException != null)
            {
                errorMessage += $"，错误原因：{innerException.Message}";
            }
            else
            {
                errorMessage += "。";
            }
            return errorMessage;
        }

        private static string GetInvalidPathMessage(this JToken jToken, string path)
        {
            return $"未找到当前路径“{GetFullPath(jToken, path)}”";
        }

        public static TData[] ToArray<TData>(this JArray jArray)
        {
            if (jArray != null)
            {
                List<TData> list = new List<TData>();
                foreach (var jItem in jArray)
                {
                    list.Add(jItem.Value<TData>());
                }
                return list.ToArray();
            }
            return null;
        }
    }
}
