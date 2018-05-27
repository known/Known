namespace Known.Web
{
    /// <summary>
    /// 压缩方法。
    /// </summary>
    public enum CompressionMethod
    {
        /// <summary>
        /// 不压缩。
        /// </summary>
        None,
        /// <summary>
        /// 自动选择。
        /// </summary>
        Automatic,
        /// <summary>
        /// GZip压缩。
        /// </summary>
        GZip,
        /// <summary>
        /// Deflate压缩。
        /// </summary>
        Deflate
    }
}
