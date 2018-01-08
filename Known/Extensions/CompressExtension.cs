using System;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;

namespace Known.Extensions
{
    /// <summary>
    /// 对象解压缩扩展类。
    /// </summary>
    public static class CompressExtension
    {
        /// <summary>
        /// 将对象压缩成字节数组。
        /// </summary>
        /// <param name="value">对象。</param>
        /// <returns>字节数组。</returns>
        public static byte[] Compress(this object value)
        {
            byte[] buffer = null;
            var bytes = value.ToBytes();
            using (var zms = new MemoryStream())
            using (var gzs = new GZipStream(zms, CompressionMode.Compress, true))
            {
                gzs.Write(bytes, 0, bytes.Length);
                gzs.Close();
                zms.Position = 0;
                buffer = new byte[zms.Length];
                zms.Read(buffer, 0, (int)zms.Length);
                zms.Flush();
                zms.Close();
            }
            return buffer;
        }

        /// <summary>
        /// 将字节数组解压成指定类型的对象。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="bytes">字节数组。</param>
        /// <returns>指定类型的对象。</returns>
        public static T Decompress<T>(this byte[] bytes)
        {
            byte[] buffer = null;
            using (var ms = new MemoryStream(bytes))
            using (var zs = new GZipStream(ms, CompressionMode.Decompress))
            {
                int offset = 0;
                while (true)
                {
                    Array.Resize<byte>(ref buffer, (offset + bytes.Length) + 1);
                    int num2 = zs.Read(buffer, offset, bytes.Length);
                    if (num2 == 0)
                        break;
                    offset += num2;
                }
                Array.Resize(ref buffer, offset);
                ms.Flush();
                ms.Close();
                zs.Close();
            }

            var bf = new BinaryFormatter();
            return (T)bf.Deserialize(new MemoryStream(buffer));
        }

        /// <summary>
        /// 将DataSet压缩成字节数组。
        /// </summary>
        /// <param name="set">DataSet对象。</param>
        /// <returns>字节数组。</returns>
        public static byte[] Compress(this DataSet set)
        {
            using (var stream = new MemoryStream())
            {
                set.WriteXml(stream, XmlWriteMode.WriteSchema);
                var ms = new MemoryStream();
                using (var ds = new DeflateStream(ms, CompressionMode.Compress, true))
                {
                    var bytes = stream.ToArray();
                    ds.Write(bytes, 0, bytes.Length);
                    ds.Close();
                    stream.Close();
                    ms.Close();
                }
                return ms.ToArray();
            }
        }

        /// <summary>
        /// 将字节数组解压成DataSet对象。
        /// </summary>
        /// <param name="buffer">字节数组。</param>
        /// <returns>DataSet对象。</returns>
        public static DataSet Decompress(this byte[] buffer)
        {
            using (var ms = new MemoryStream())
            {
                ms.Write(buffer, 0, buffer.Length);
                ms.Position = 0;
                using (var ds = new DeflateStream(ms, CompressionMode.Decompress))
                {
                    var set = new DataSet();
                    set.ReadXml(ds);
                    ms.Close();
                    ds.Close();
                    return set;
                }
            }
        }
    }
}
