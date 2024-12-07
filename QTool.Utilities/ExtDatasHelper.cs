using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTool
{
    public static class ExtDatasHelper
    {


        public static string GetExtValue(this IExtDatas data, ExtDatasKeys key)
        {
            if (data.ExtDatas != null && data.ExtDatas.TryGetValue((byte)key, out string val))
            {
                return val;
            }
            else
            {
                return null;
            }
        }

        public static void SetExtValue(this IExtDatas data, ExtDatasKeys key, string value)
        {
            if (data.ExtDatas == null)
            {
                data.ExtDatas = new Dictionary<byte, string>();
            }

            data.ExtDatas[(byte)key] = value;
        }
    }
}
