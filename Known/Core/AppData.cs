using System.IO.Compression;

namespace Known.Core;

/// <summary>
/// 框架配置数据库文件。
/// </summary>
public sealed class AppData
{
    private static readonly string KmdPath = "./AppData.kmd";

    private AppData() { }

    /// <summary>
    /// 取得框架配置数据信息。
    /// </summary>
    public static AppDataInfo Data { get; private set; } = new();

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
    public static Func<byte[], AppDataInfo> OnParseData { get; set; }

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
        DataHelper.Initialize(modules);
        SaveData();
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

    /// <summary>
    /// 根据ID获取实体插件信息。
    /// </summary>
    /// <param name="id">模块ID。</param>
    /// <returns>实体插件信息。</returns>
    public static EntityPluginInfo GetEntityPlugin(string id)
    {
        var module = GetModule(id);
        if (module == null || module.Plugins == null)
            return null;

        return module.Plugins.GetPlugin<EntityPluginInfo>();
    }

    #region TopNav
    internal static Task<Result> SaveTopNavsAsync(List<PluginInfo> infos)
    {
        Data ??= new AppDataInfo();
        Data.TopNavs = infos;
        SaveData();
        return Result.SuccessAsync("保存成功！");
    }
    #endregion

    #region Language
    internal static Task<Result> DeleteLanguagesAsync(List<LanguageInfo> infos)
    {
        foreach (var info in infos)
        {
            var item = Data.Languages.FirstOrDefault(b => b.Id == info.Id);
            if (item != null)
                Data.Languages.Remove(item);
        }
        SaveData();
        return Result.SuccessAsync("删除成功！");
    }

    internal static Task<Result> SaveLanguageAsync(LanguageInfo info)
    {
        Data ??= new AppDataInfo();
        var item = Data.Languages.FirstOrDefault(b => b.Id == info.Id);
        if (item == null)
        {
            item = new LanguageInfo();
            Data.Languages.Add(item);
        }
        item.Id = info.Id;
        item.Name = info.Name;
        item.Icon = info.Icon;
        SaveData();
        return Result.SuccessAsync("保存成功！");
    }
    #endregion

    #region Button
    internal static Task<Result> DeleteButtonsAsync(List<ButtonInfo> infos)
    {
        foreach (var info in infos)
        {
            var item = Data.Buttons.FirstOrDefault(b => b.Id == info.Id);
            if (item != null)
                Data.Buttons.Remove(item);
        }
        SaveData();
        return Result.SuccessAsync("删除成功！");
    }

    internal static Task<Result> SaveButtonAsync(ButtonInfo info)
    {
        Data ??= new AppDataInfo();
        var item = Data.Buttons.FirstOrDefault(b => b.Id == info.Id);
        if (item == null)
        {
            item = new ButtonInfo();
            Data.Buttons.Add(item);
        }
        item.Id = info.Id;
        item.Name = info.Name;
        item.Icon = info.Icon;
        item.Style = info.Style;
        item.Position = info.Position;
        SaveData();
        return Result.SuccessAsync("保存成功！");
    }
    #endregion

    #region Menu
    internal static Task<Result> DeleteMenuAsync(MenuInfo info)
    {
        var module = GetModule(info.Id);
        if (module == null)
            return Result.ErrorAsync("模块不存在！");

        Modules?.Remove(module);
        var modules = Modules.Where(m => m.ParentId == info.ParentId).OrderBy(m => m.Sort).ToList();
        modules?.Resort();
        SaveData();
        return Result.SuccessAsync("删除成功！");
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
        module.Sort = info.Sort;
        module.Plugins = info.Plugins;
        SaveData();
        return Result.SuccessAsync("保存成功！", info);
    }
    #endregion

    #region Private
    internal static void LoadData()
    {
        if (!Enabled)
            return;

        if (!File.Exists(KmdPath))
            return;

        var bytes = File.ReadAllBytes(KmdPath);
        if (OnParseData != null)
            Data = OnParseData(bytes);
        else
            Data = ParseData(bytes);
    }

    private static void SaveData()
    {
        if (!Enabled)
            return;

        var bytes = OnFormatData != null
                  ? OnFormatData(Data)
                  : FormatData(Data);
        File.WriteAllBytes(KmdPath, bytes);
    }

    private static AppDataInfo ParseData(byte[] bytes)
    {
        using (var stream = new MemoryStream(bytes))
        using (var reader = new MemoryStream())
        using (var gzip = new GZipStream(stream, CompressionMode.Decompress))
        {
            gzip.CopyTo(reader);
            var json = Encoding.UTF8.GetString(reader.ToArray());
            return Utils.FromJson<AppDataInfo>(json);
        }
    }

    private static byte[] FormatData(AppDataInfo data)
    {
        var json = Utils.ToJson(data);
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
    #endregion
}