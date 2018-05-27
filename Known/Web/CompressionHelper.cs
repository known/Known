using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace Known.Web
{
    /// <summary>
    /// 解压缩帮助者。
    /// </summary>
    public sealed class CompressionHelper
    {
        /// <summary>
        /// 将字节进行Deflate压缩。
        /// </summary>
        /// <param name="bytes">压缩前字节。</param>
        /// <returns>压缩后字节。</returns>
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

        /// <summary>
        /// 异步进行Deflate解压。
        /// </summary>
        /// <param name="stream">待解压流。</param>
        /// <param name="destination">目标流。</param>
        /// <returns>异步操作的任务对象。</returns>
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

        /// <summary>
        /// 将字节进行GZip压缩。
        /// </summary>
        /// <param name="bytes">压缩前字节。</param>
        /// <returns>压缩后字节。</returns>
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

        /// <summary>
        /// 异步进行GZip解压。
        /// </summary>
        /// <param name="stream">待解压流。</param>
        /// <param name="destination">目标流。</param>
        /// <returns>异步操作的任务对象。</returns>
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
    }
}
