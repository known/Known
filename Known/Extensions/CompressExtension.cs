using System;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;

namespace Known.Extensions
{
    public static class CompressExtension
    {
        public static byte[] Compress(this object value)
        {
            if (value == null)
                return null;

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

        public static T Decompress<T>(this byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
                return default(T);

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

        public static byte[] Compress(this DataSet set)
        {
            if (set == null)
                return null;

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

        public static DataSet Decompress(this byte[] buffer)
        {
            if (buffer == null || buffer.Length == 0)
                return null;

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
