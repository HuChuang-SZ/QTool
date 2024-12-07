using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTool
{
    public static class JsonHelper
    {
        public static TData[] ToArray<TData>(this JToken jToken, string key)
        {
            var jArray = jToken.Value<JArray>(key);
            if (jArray != null)
            {
                var datas = new TData[jArray.Count];
                for (int i = 0; i < jArray.Count; i++)
                {
                    datas[i] = jArray[i].Value<TData>();
                }
                return datas;
            }
            else
            {
                return Array.Empty<TData>();
            }
        }
    }
}
