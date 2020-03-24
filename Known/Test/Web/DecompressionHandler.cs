using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Known.Helpers;

namespace Known.Web
{
    /// <summary>
    /// 内容解压处理者类。
    /// </summary>
    public class DecompressionHandler : DelegatingHandler
    {
        /// <summary>
        /// 异步发送处理请求。
        /// </summary>
        /// <param name="request">请求对象。</param>
        /// <param name="cancellationToken">取消操作通知。</param>
        /// <returns>异步结果。</returns>
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
