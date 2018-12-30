using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Known.Web
{
    public class DecompressionHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.Method.Method == "POST")
            {
                var encodings = request.Content.Headers.ContentEncoding
                    .Where(e => e == "gzip" || e == "deflate")
                    .ToList();

                if (encodings.Count > 0)
                {
                    var content = request.Content;
                    var decompressedStream = new MemoryStream();

                    if (encodings.First() == "gzip")
                    {
                        await CompressionHelper.GZipDecompressAsync(await content.ReadAsStreamAsync(), decompressedStream);
                    }
                    else
                    {
                        await CompressionHelper.DeflateDecompressAsync(await content.ReadAsStreamAsync(), decompressedStream);
                    }

                    if (decompressedStream.CanSeek)
                    {
                        decompressedStream.Seek(0, SeekOrigin.Begin);
                    }
                    request.Content = new StreamContent(decompressedStream);
                    CopyHeaders(content, request.Content);
                }
            }

            return await base.SendAsync(request, cancellationToken);
        }

        private static void CopyHeaders(HttpContent source, HttpContent target)
        {
            foreach (var item in source.Headers)
            {
                target.Headers.TryAddWithoutValidation(item.Key, item.Value);
            }
        }
    }
}
