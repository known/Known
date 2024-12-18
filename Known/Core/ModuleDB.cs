using System.IO.Compression;

namespace Known.Core;

/// <summary>
/// 模块配置数据库文件。
/// </summary>
public sealed class ModuleDB
{
    private static readonly string KmdPath = "./AppData.kmd";

    private ModuleDB() { }

    /// <summary>
    /// 取得或设置是否启用配置文件存储，默认启用。
    /// </summary>
    public static bool IsAppData { get; set; } = true;

    /// <summary>
    /// 取得系统配置的模块列表。
    /// </summary>
    public static List<ModuleInfo> Modules { get; internal set; } = [];

    /// <summary>
    /// 取得或设置解析配置数据委托。
    /// </summary>
    public static Func<byte[], List<ModuleInfo>> OnParseData { get; set; }

    /// <summary>
    /// 取得或设置格式化配置数据委托。
    /// </summary>
    public static Func<List<ModuleInfo>, byte[]> OnFormatData { get; set; }

    /// <summary>
    /// 初始化模块数据库。
    /// </summary>
    /// <param name="modules">系统模块列表。</param>
    public static void Initialize(List<ModuleInfo> modules)
    {
        Modules = modules;
        Save();
    }

    /// <summary>
    /// 根据ID获取模块信息。
    /// </summary>
    /// <param name="id">模块ID。</param>
    /// <returns>模块信息。</returns>
    public static ModuleInfo GetModule(string id)
    {
        return Modules.FirstOrDefault(m => m.Id == id);
    }

    internal static void Load()
    {
        if (!IsAppData)
            return;

        if (!File.Exists(KmdPath))
            return;

        var bytes = File.ReadAllBytes(KmdPath);
        if (OnParseData != null)
            Modules = OnParseData(bytes);
        else
            Modules = ParseData(bytes);
    }

    internal static void Save()
    {
        if (!IsAppData)
            return;

        var bytes = OnFormatData != null
                  ? OnFormatData(Modules)
                  : FormatData(Modules);
        File.WriteAllBytes(KmdPath, bytes);
    }

    private static List<ModuleInfo> ParseData(byte[] bytes)
    {
        using (var stream = new MemoryStream(bytes))
        using (var reader = new MemoryStream())
        using (var gzip = new GZipStream(stream, CompressionMode.Decompress))
        {
            gzip.CopyTo(reader);
            var json = Encoding.UTF8.GetString(reader.ToArray());
            return Utils.FromJson<List<ModuleInfo>>(json);
        }
    }

    private static byte[] FormatData(List<ModuleInfo> modules)
    {
        var json = Utils.ToJson(modules);
        var bytes = Encoding.UTF8.GetBytes(json);
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
}