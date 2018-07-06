using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace Known.Web
{
    public sealed class CompressionHelper
    {
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
