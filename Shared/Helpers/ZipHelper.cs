using System.IO.Compression;

namespace Known.Helpers;

/// <summary>
/// Zip压缩帮助者类。
/// </summary>
public sealed class ZipHelper
{
    private ZipHelper() { }

    /// <summary>
    /// 将泛型对象压缩成GZip格式字节字符串。
    /// </summary>
    /// <param name="value">泛型对象。</param>
    /// <returns>GZip格式字节字符串。</returns>
    public static string ZipDataAsString<T>(T value)
    {
        if (value == null)
            return string.Empty;

        var bytes = ZipData(value);
        return Convert.ToBase64String(bytes);
    }

    /// <summary>
    /// 异步将字GZip字节数组解压成原始泛型对象。
    /// </summary>
    /// <param name="value">GZip字节。</param>
    /// <returns>原始泛型对象。</returns>
    public static T UnZipDataFromString<T>(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return default;

        var bytes = Convert.FromBase64String(value);
        return UnZipData<T>(bytes);
    }

    /// <summary>
    /// 异步将泛型对象压缩成GZip格式字节流。
    /// </summary>
    /// <param name="value">泛型对象。</param>
    /// <returns>GZip格式字节流。</returns>
    public static byte[] ZipData<T>(T value)
    {
        var data = Utils.ToJson(value);
        return ZipData(data);
    }

    /// <summary>
    /// 异步将泛型对象压缩成GZip格式字节流。
    /// </summary>
    /// <typeparam name="T">对象类型。</typeparam>
    /// <param name="value">泛型对象。</param>
    /// <returns>GZip格式字节流。</returns>
    public static Task<byte[]> ZipDataAsync<T>(T value)
    {
        var data = Utils.ToJson(value);
        return ZipDataAsync(data);
    }

    /// <summary>
    /// 将字符串压缩成GZip格式字节流。
    /// </summary>
    /// <param name="data">字符串。</param>
    /// <returns>字节流。</returns>
    public static byte[] ZipData(string data)
    {
        if (string.IsNullOrWhiteSpace(data))
            return null;

        var bytes = Encoding.UTF8.GetBytes(data);
        using (var stream = new MemoryStream())
        using (var gzip = new GZipStream(stream, CompressionMode.Compress, true))
        {
            gzip.Write(bytes, 0, bytes.Length);
            gzip.Flush();
            stream.Position = 0;

            var buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            return buffer;
        }
    }

    /// <summary>
    /// 异步将字符串压缩成GZip格式字节流。
    /// </summary>
    /// <param name="data">字符串。</param>
    /// <returns>字节流。</returns>
    public static async Task<byte[]> ZipDataAsync(string data)
    {
        if (string.IsNullOrWhiteSpace(data))
            return null;

        var bytes = Encoding.UTF8.GetBytes(data);
        using (var stream = new MemoryStream())
        using (var gzip = new GZipStream(stream, CompressionMode.Compress, true))
        {
            await gzip.WriteAsync(bytes, 0, bytes.Length);
            await gzip.FlushAsync();
            stream.Position = 0;

            var buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            return buffer;
        }
    }

    /// <summary>
    /// 异步将字GZip字节数组解压成原始泛型对象。
    /// </summary>
    /// <param name="bytes">GZip字节。</param>
    /// <returns>原始泛型对象。</returns>
    public static T UnZipData<T>(byte[] bytes)
    {
        var json = UnZipData(bytes);
        return Utils.FromJson<T>(json);
    }

    /// <summary>
    /// 异步将字GZip字节数组解压成原始泛型对象。
    /// </summary>
    /// <typeparam name="T">对象类型。</typeparam>
    /// <param name="bytes">GZip字节。</param>
    /// <returns>原始泛型对象。</returns>
    public static async Task<T> UnZipDataAsync<T>(byte[] bytes)
    {
        var json = await UnZipDataAsync(bytes);
        return Utils.FromJson<T>(json);
    }

    /// <summary>
    /// 将字GZip字节数组解压成原始字符串。
    /// </summary>
    /// <param name="bytes">GZip字节。</param>
    /// <returns>原始字符串。</returns>
    public static string UnZipData(byte[] bytes)
    {
        if (bytes == null || bytes.Length == 0)
            return string.Empty;

        using (var stream = new MemoryStream(bytes))
        using (var reader = new MemoryStream())
        using (var gzip = new GZipStream(stream, CompressionMode.Decompress))
        {
            gzip.CopyTo(reader);
            return Encoding.UTF8.GetString(reader.ToArray());
        }
    }

    /// <summary>
    /// 异步将字GZip字节数组解压成原始字符串。
    /// </summary>
    /// <param name="bytes">GZip字节。</param>
    /// <returns>原始字符串。</returns>
    public static async Task<string> UnZipDataAsync(byte[] bytes)
    {
        if (bytes == null || bytes.Length == 0)
            return string.Empty;

        using (var stream = new MemoryStream(bytes))
        using (var reader = new MemoryStream())
        using (var gzip = new GZipStream(stream, CompressionMode.Decompress))
        {
            await gzip.CopyToAsync(reader);
            return Encoding.UTF8.GetString(reader.ToArray());
        }
    }
}