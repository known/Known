using System.IO.Compression;

namespace Known.Core;

/// <summary>
/// 框架配置数据库文件。
/// </summary>
public sealed class AppData
{
    private static readonly string KmdPath = "./AppData.kmd";

    private AppData() { }

    internal static AppDataInfo Data { get; set; } = new();

    /// <summary>
    /// 取得或设置是否启用配置文件存储，默认启用。
    /// </summary>
    public static bool Enabled { get; set; } = true;

    /// <summary>
    /// 取得系统配置的模块列表。
    /// </summary>
    public static List<ModuleInfo> Modules => Data?.Modules;

    /// <summary>
    /// 取得或设置解析配置数据委托。
    /// </summary>
    public static Action<byte[]> OnParseData { get; set; }

    /// <summary>
    /// 取得或设置格式化配置数据委托。
    /// </summary>
    public static Func<AppDataInfo, byte[]> OnFormatData { get; set; }

    /// <summary>
    /// 初始化模块数据库。
    /// </summary>
    /// <param name="modules">系统模块列表。</param>
    public static void Initialize(List<ModuleInfo> modules)
    {
        Data ??= new AppDataInfo();
        Data.Modules = modules;
        Save();
    }

    /// <summary>
    /// 根据ID获取模块信息。
    /// </summary>
    /// <param name="id">模块ID。</param>
    /// <returns>模块信息。</returns>
    public static ModuleInfo GetModule(string id)
    {
        return Modules?.FirstOrDefault(m => m.Id == id);
    }

    internal static Task<Result> SaveTopNavsAsync(List<TopNavInfo> infos)
    {
        Data ??= new AppDataInfo();
        Data.TopNavs = infos;
        Save();
        return Result.SuccessAsync("保存成功！");
    }

    internal static Task<Result> SaveMenuAsync(MenuInfo info)
    {
        var module = GetModule(info.Id);
        if (module == null)
        {
            module = new ModuleInfo();
            if (Modules == null)
            {
                Data ??= new AppDataInfo();
                Data.Modules = [];
            }
            Modules.Add(module);
        }
        module.Id = info.Id;
        module.ParentId = info.ParentId;
        module.Name = info.Name;
        module.Icon = info.Icon;
        module.Type = info.Type;
        module.Target = info.Target;
        module.Url = info.Url;
        module.PluginId = info.PluginId;
        module.Parameters = info.Parameters;
        Save();
        return Result.SuccessAsync("保存成功！", info);
    }

    internal static void Load()
    {
        if (!Enabled)
            return;

        if (!File.Exists(KmdPath))
            return;

        var bytes = File.ReadAllBytes(KmdPath);
        if (OnParseData != null)
            OnParseData(bytes);
        else
            ParseData(bytes);
    }

    internal static void Save()
    {
        if (!Enabled)
            return;

        var bytes = OnFormatData != null
                  ? OnFormatData(Data)
                  : FormatData();
        File.WriteAllBytes(KmdPath, bytes);
    }

    private static void ParseData(byte[] bytes)
    {
        using (var stream = new MemoryStream(bytes))
        using (var reader = new MemoryStream())
        using (var gzip = new GZipStream(stream, CompressionMode.Decompress))
        {
            gzip.CopyTo(reader);
            var json = Encoding.UTF8.GetString(reader.ToArray());
            Data = Utils.FromJson<AppDataInfo>(json);
        }
    }

    private static byte[] FormatData()
    {
        var json = Utils.ToJson(Data);
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