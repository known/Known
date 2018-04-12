using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Known.Extensions;

namespace Known.Web
{
    /// <summary>
    /// Http压缩内容。
    /// </summary>
    public class CompressedContent : HttpContent
    {
        private HttpContent content;
        private CompressionMethod method;

        /// <summary>
        /// 构造函数，创建一个Http压缩内容实例。
        /// </summary>
        /// <param name="content">Http内容。</param>
        /// <param name="method">压缩方法。</param>
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
                Headers.ContentEncoding.Add(method.GetName().ToLower());
            }
        }

        /// <summary>
        /// 将Http内容序列化到流，此为异步操作。
        /// </summary>
        /// <param name="stream">目标流。</param>
        /// <param name="context">传输上下文。</param>
        /// <returns>异步操作的任务对象。</returns>
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

        /// <summary>
        /// 确定Http内容是否具有有效的长度（以字节为单位）。
        /// </summary>
        /// <param name="length">Http内容的长度（以字节为单位）。</param>
        /// <returns>如果length是有效长度，则为true；否则为false。</returns>
        protected override bool TryComputeLength(out long length)
        {
            length = -1;
            return false;
        }
    }
}
