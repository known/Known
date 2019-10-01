using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;

namespace Known.Helpers
{
    public sealed class CompressionHelper
    {
        #region DeflateCompress
        public static string DeflateCompress(string data)
        {
            if (string.IsNullOrWhiteSpace(data))
                return string.Empty;

            var bytes = Encoding.UTF8.GetBytes(data);
            var compressedBytes = DeflateCompress(bytes);
            return Convert.ToBase64String(compressedBytes);
        }

        public static byte[] DeflateCompress(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
                return bytes;

            try
            {
                using (var stream = new MemoryStream())
                {
                    using (var deflateStream = new DeflateStream(stream, CompressionMode.Compress))
                    {
                        deflateStream.Write(bytes, 0, bytes.Length);
                    }
                    return stream.ToArray();
                }
            }
            catch
            {
                return bytes;
            }
        }
        #endregion

        #region DeflateDecompress
        public static string DeflateDecompress(string data)
        {
            if (string.IsNullOrWhiteSpace(data))
                return string.Empty;

            var bytes = Convert.FromBase64String(data);
            var decompressedBytes = DeflateDecompress(bytes);
            return Encoding.UTF8.GetString(decompressedBytes);
        }

        public static byte[] DeflateDecompress(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
                return bytes;

            try
            {
                using (var stream = new MemoryStream(bytes))
                using (var deflateStream = new DeflateStream(stream, CompressionMode.Decompress))
                using (var outStream = new MemoryStream())
                {
                    var buffer = new byte[1024];
                    while (true)
                    {
                        var count = deflateStream.Read(buffer, 0, buffer.Length);
                        if (count <= 0)
                            break;

                        outStream.Write(buffer, 0, count);
                    }

                    return outStream.ToArray();
                }
            }
            catch
            {
                return bytes;
            }
        }

        public static async Task DeflateDecompressAsync(Stream stream, Stream destination)
        {
            try
            {
                using (var deflateStream = new DeflateStream(stream, CompressionMode.Decompress))
                {
                    await deflateStream.CopyToAsync(destination);
                }
            }
            catch { }
        }
        #endregion

        #region GZipCompress
        public static string GZipCompress(string data)
        {
            if (string.IsNullOrWhiteSpace(data))
                return string.Empty;

            var bytes = Encoding.UTF8.GetBytes(data);
            var compressedBytes = GZipCompress(bytes);
            return Convert.ToBase64String(compressedBytes);
        }

        public static byte[] GZipCompress(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
                return bytes;

            try
            {
                using (var stream = new MemoryStream())
                {
                    using (var gZipStream = new GZipStream(stream, CompressionMode.Compress))
                    {
                        gZipStream.Write(bytes, 0, bytes.Length);
                    }
                    return stream.ToArray();
                }
            }
            catch
            {
                return bytes;
            }
        }
        #endregion

        #region GZipDecompress
        public static string GZipDecompress(string data)
        {
            if (string.IsNullOrWhiteSpace(data))
                return string.Empty;

            var bytes = Convert.FromBase64String(data);
            var decompressedBytes = GZipDecompress(bytes);
            return Encoding.UTF8.GetString(decompressedBytes);
        }

        public static byte[] GZipDecompress(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
                return bytes;

            try
            {
                using (var stream = new MemoryStream(bytes))
                using (var gZipStream = new GZipStream(stream, CompressionMode.Decompress))
                using (var outStream = new MemoryStream())
                {
                    var buffer = new byte[1024];
                    while (true)
                    {
                        var count = gZipStream.Read(buffer, 0, buffer.Length);
                        if (count <= 0)
                            break;

                        outStream.Write(buffer, 0, count);
                    }

                    return outStream.ToArray();
                }
            }
            catch
            {
                return bytes;
            }
        }

        public static async Task GZipDecompressAsync(Stream stream, Stream destination)
        {
            try
            {
                using (var gZipStream = new GZipStream(stream, CompressionMode.Decompress))
                {
                    await gZipStream.CopyToAsync(destination);
                }
            }
            catch { }
        }
        #endregion
    }
}
