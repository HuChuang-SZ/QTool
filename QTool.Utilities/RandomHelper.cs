using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTool
{
    public static class RandomHelper
    {
        private readonly static Random _random = new Random();

        /// <summary>
        /// 随机验证码
        /// </summary>
        /// <returns></returns>
        public static int GetVerifyCode()
        {
            return _random.Next(100000, 999999);
        }
    }
}
