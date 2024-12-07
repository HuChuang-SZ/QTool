using System;
using System.Security.Cryptography;
using System.Text;

namespace QTool
{
    public class RSAHelper
    {
        const string PublicKey = @"<RSAKeyValue><Modulus>3/BkzQY+XhrzTFUBHnfDtD5pTrbccPJt5mugLnu/3KdKa9GcE7kf5cbdNWjWTFh/dCYfIx5aB7gRfPu+RSF1FxkYe96IHu5Nww+u9t7v4xpF6oBkiEmPQ1f6qk9qacXpj1il3b3iXs6F/04/qWexrBF5G4wxM73HuykH2WMgqdU=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

        const string PrivateKey = @"<RSAKeyValue><Modulus>3/BkzQY+XhrzTFUBHnfDtD5pTrbccPJt5mugLnu/3KdKa9GcE7kf5cbdNWjWTFh/dCYfIx5aB7gRfPu+RSF1FxkYe96IHu5Nww+u9t7v4xpF6oBkiEmPQ1f6qk9qacXpj1il3b3iXs6F/04/qWexrBF5G4wxM73HuykH2WMgqdU=</Modulus><Exponent>AQAB</Exponent><P>5OMnhyL2NliKElBF8O18dx1MY2gVHrbcRbKRb9P0q3vdgkyQxtbzba/Rb0WX/VGSpmnjEWm0Kz/0WnsgL8XHuw==</P><Q>+ncvp732eoVUGWhMnJR7vPjKwuaZDbycOOpVzRfF/CZtgPZbprLR5UgJCNGeRMrOzGzeoFJU43n6SjgVHgrTrw==</Q><DP>nF9sk/CY0Ywv2E8rWWyGikJj+84SD8fSOXPD1Oiz0axwKyLwDkiwSpHBvgmHyzM9w5+32B+lTZ8F7AMOfFTdHQ==</DP><DQ>3QD2B+lS5+Flx9WEZFWQqbpOD5QyUTCwGHiNpR7pEAlIbpbzYDBBjTL25dUw9jqOJ0uZVZXQWbhz5bF+0Ld0+Q==</DQ><InverseQ>T+z5Rh9Vs+wyEpuVeCE+Dz9h1WN/kDrzBHjYQX9lWTgpszz1RWKsTZSz+ImlcrmmzYIuxMkcUqLROdeXiJi6TA==</InverseQ><D>mYEz18a74c4fkOeECNqX8GoHzclTeqjz5MtJ1hNGBX83aIkar/pqfxu+buoaW+MeGwQ2u+AGXtwyQLZgSwsQMyiNyqANwEaU+RW6xDF9Y1O+3OYXrJffgUFXIG0BIhFjUNgnJruowC0N1vpmZ+gPWkM7jtRNFKFCmDrtujxR0FE=</D></RSAKeyValue>";


        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public byte[] Encrypt(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            if (data.Length == 0)
                throw new ArgumentException("数组长度必须大于0。");

            if (data.Length > 117)
                throw new ArgumentOutOfRangeException("数组长度必须小于等于117。");

            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(1024))
            {
                //将公钥导入到RSA对象中，准备加密；
                rsa.FromXmlString(PublicKey);

                //对数据data进行加密，并返回加密结果；
                //第二个参数用来选择Padding的格式
                byte[] buffer = rsa.Encrypt(data, false);
                return buffer;
            }
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public string Encrypt(string data)
        {
            return Convert.ToBase64String(Encrypt(Encoding.UTF8.GetBytes(data)));
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public byte[] Decrypt(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            if (data.Length == 0)
                throw new ArgumentException("数组长度必须大于0。");

            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(1024))
            {
                //将私钥导入RSA中，准备解密；
                rsa.FromXmlString(PrivateKey);

                //对数据进行解密，并返回解密结果；
                return rsa.Decrypt(data, false);
            }
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public string Decrypt(string base64Str)
        {
            return Encoding.UTF8.GetString(Decrypt(Convert.FromBase64String(base64Str)));
        }
    }
}
