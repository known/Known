using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Known.Web
{
    public class CompressedContent : HttpContent
    {
        private HttpContent content;
        private readonly CompressionMethod method;

        public CompressedContent(HttpContent content, CompressionMethod method)
        {
            this.content = content;
            this.method = method;

            foreach (var item in content.Headers)
            {
                Headers.TryAddWithoutValidation(item.Key, item.Value);
            }

            if (method == CompressionMethod.GZip || method == CompressionMethod.Deflate)
            {
                var item = Enum.GetName(typeof(CompressionMethod), method).ToLower();
                Headers.ContentEncoding.Add(item);
            }
        }

        protected async override Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            if (method == CompressionMethod.GZip)
            {
                using (var gzipStream = new GZipStream(stream, CompressionMode.Compress, true))
                {
                    await content.CopyToAsync(gzipStream);
                }
            }
            else if (method == CompressionMethod.Deflate)
            {
                using (var deflateStream = new DeflateStream(stream, CompressionMode.Compress, true))
                {
                    await content.CopyToAsync(deflateStream);
                }
            }

            await content.CopyToAsync(stream);
        }

        protected override bool TryComputeLength(out long length)
        {
            length = -1;
            return false;
        }
    }
}
