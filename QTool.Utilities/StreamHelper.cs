using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace QTool
{
    public static class StreamHelper
    {
        /// <summary>
        /// 读取流，返回bytes值
        /// </summary>
        /// <param name="stream">文件流</param>
        /// <param name="position">位置</param>
        /// <param name="length">长度</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static byte[] ReadBytes(this Stream stream, long position, long length)
        {
            //if (stream == null)
            //    throw new ArgumentNullException("stream");

            //if (position < 0)
            //    throw new ArgumentOutOfRangeException("position");

            //if (length < 0)
            //    throw new ArgumentOutOfRangeException("length");

            //if (position + length > stream.Length)
            //    throw new ArgumentException("读取数据超过流最大长度。");

            stream.Position = position;
            var buffer = new byte[length];

            if (stream.Read(buffer, 0, buffer.Length) != buffer.Length)
            {
                throw new Exception("仅读取到部分数据");
            }

            return buffer;
        }

        /// <summary>
        /// 将bytes数据添加到流末尾
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="bytes"></param>
        public static void WriteBytes(this Stream stream, byte[] bytes)
        {
            stream.Write(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// 读取流片段数据
        /// </summary>
        /// <param name="stream">文件流</param>
        /// <param name="partCnt">片段数</param>
        /// <param name="partLength">单个片段长度</param>
        /// <returns></returns>
        public static byte[] ReadPartBytes(this Stream stream, int partCnt, int partLength)
        {
            if (stream.Length < partLength * partCnt)
            {
                return stream.ReadBytes(0, stream.Length);
            }
            else
            {
                using (var ms = new MemoryStream())
                {
                    ms.Write(BitConverter.GetBytes(stream.Length), 0, 8);

                    for (int i = 0; i < partCnt; i++)
                    {
                        ms.WriteBytes(stream.ReadBytes(stream.Length / partCnt * i, partLength));
                    }

                    ms.WriteBytes(stream.ReadBytes(stream.Length - partLength, partLength));

                    return ms.ToArray();
                }
            }
        }

        /// <summary>
        /// 读取文件片段数据
        /// </summary>
        /// <param name="file">文件</param>
        /// <param name="partCnt">片段数</param>
        /// <param name="partLength">单个片段长度</param>
        /// <returns></returns>
        public static byte[] ReadPartBytes(this string file, int partCnt, int partLength)
        {
            using (var stream = File.OpenRead(file))
            {
                return stream.ReadPartBytes(partCnt, partLength);
            }
        }
    }
}
