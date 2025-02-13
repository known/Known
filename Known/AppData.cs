using System.IO.Compression;

namespace Known;

/// <summary>
/// 框架配置数据库文件。
/// </summary>
public sealed class AppData
{
    private static readonly string KmdPath = "./AppData.kmd";
    private static readonly string KdbPath = "./AppData.kdb";

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
    /// 取得或设置解析配置数据委托。
    /// </summary>
    public static Func<byte[], AppDataInfo> OnParseData { get; set; }

    /// <summary>
    /// 取得或设置格式化配置数据委托。
    /// </summary>
    public static Func<AppDataInfo, byte[]> OnFormatData { get; set; }

    /// <summary>
    /// 根据ID获取模块信息。
    /// </summary>
    /// <param name="id">模块ID。</param>
    /// <returns>模块信息。</returns>
    public static ModuleInfo GetModule(string id)
    {
        return Data.Modules?.FirstOrDefault(m => m.Id == id);
    }

    /// <summary>
    /// 创建实体表格页面插件配置信息。
    /// </summary>
    /// <param name="pageType">页面组件类型。</param>
    /// <returns>插件配置信息。</returns>
    public static TablePageInfo CreateTablePage(Type pageType)
    {
        if (TypeHelper.IsSubclassOfGeneric(pageType, typeof(BaseTablePage<>), out var types))
            return AppDefaultData.CreateTablePage(pageType, types[0]);

        return null;
    }

    /// <summary>
    /// 根据ID获取实体插件参数配置信息。
    /// </summary>
    /// <param name="id">模块ID。</param>
    /// <returns>实体插件参数配置信息。</returns>
    public static TablePageInfo GetTablePageParameter(string id)
    {
        var module = GetModule(id);
        if (module == null || module.Plugins == null)
            return null;

        return module.Plugins.GetPluginParameter<TablePageInfo>();
    }

    #region AppData
    internal static void LoadAppData()
    {
        if (!Enabled)
            return;

        if (!File.Exists(KmdPath))
        {
            AppDefaultData.Load(Data);
            return;
        }

        var bytes = File.ReadAllBytes(KmdPath);
        if (OnParseData != null)
            Data = OnParseData(bytes);
        else
            Data = ParseData<AppDataInfo>(bytes);

        // 加载新增菜单页面
        AppDefaultData.Load(Data);
    }

    /// <summary>
    /// 恢复配置数据。
    /// </summary>
    /// <param name="kmdPath">配置文件路径。</param>
    public static void Restore(string kmdPath)
    {
        if (!Enabled)
            return;

        Utils.CopyFile(kmdPath, KmdPath);
        LoadAppData();
    }

    /// <summary>
    /// 保存配置数据。
    /// </summary>
    public static void SaveData()
    {
        if (!Enabled)
            return;

        var bytes = OnFormatData != null
                  ? OnFormatData(Data)
                  : FormatData(Data);
        File.WriteAllBytes(KmdPath, bytes);
    }
    #endregion

    #region BizData
    private static Dictionary<string, string> BizData { get; set; } = [];

    internal static void LoadBizData()
    {
        if (!File.Exists(KdbPath))
            return;

        var bytes = File.ReadAllBytes(KdbPath);
        BizData = ParseData<Dictionary<string, string>>(bytes);
    }

    /// <summary>
    /// 分页查询业务数据。
    /// </summary>
    /// <typeparam name="T">数据类型。</typeparam>
    /// <param name="key">数据存储键。</param>
    /// <param name="criteria">查询条件。</param>
    /// <param name="filter">查询过滤。</param>
    /// <returns></returns>
    public static PagingResult<T> QueryModels<T>(string key, PagingCriteria criteria, Func<List<T>, List<T>> filter = null)
    {
        var datas = GetBizData<List<T>>(key) ?? [];
        if (filter != null)
            datas = filter.Invoke(datas);
        return datas == null ? new PagingResult<T>(0, []) : datas.ToPagingResult(criteria);
    }

    /// <summary>
    /// 删除业务数据。
    /// </summary>
    /// <typeparam name="T">数据类型。</typeparam>
    /// <param name="key">数据存储键。</param>
    /// <param name="infos">业务数据列表。</param>
    /// <returns></returns>
    public static Result DeleteModels<T>(string key, List<T> infos)
    {
        if (!typeof(T).HasProperty("Id"))
            return Result.Error("The model must have an Id property.");

        var datas = GetBizData<List<T>>(key) ?? [];
        foreach (var info in infos)
        {
            var id = info.Property("Id");
            var item = datas.FirstOrDefault(d => d.Property("Id").Equals(id));
            if (item != null)
                datas.Remove(item);
        }
        return SaveBizData(key, datas);
    }

    /// <summary>
    /// 保存业务数据。
    /// </summary>
    /// <typeparam name="T">数据类型。</typeparam>
    /// <param name="key">数据存储键。</param>
    /// <param name="info">业务数据。</param>
    /// <returns></returns>
    public static Result SaveModel<T>(string key, T info)
    {
        if (!typeof(T).HasProperty("Id"))
            return Result.Error("The model must have an Id property.");

        var id = info.Property("Id");
        var datas = GetBizData<List<T>>(key) ?? [];
        var item = datas.FirstOrDefault(d => d.Property("Id").Equals(id));
        if (item != null)
            datas.Remove(item);
        datas.Add(info);
        return SaveBizData(key, datas);
    }

    internal static T GetBizData<T>(string key)
    {
        if (BizData.TryGetValue(key, out var value))
            return Utils.FromJson<T>(value);

        return default;
    }

    internal static Result SaveBizData(string key, object value)
    {
        BizData[key] = Utils.ToJson(value);
        var bytes = FormatData(BizData);
        File.WriteAllBytes(KdbPath, bytes);
        return Result.Success("Save successful!");
    }
    #endregion

    #region Private
    private static T ParseData<T>(byte[] bytes)
    {
        using (var stream = new MemoryStream(bytes))
        using (var reader = new MemoryStream())
        using (var gzip = new GZipStream(stream, CompressionMode.Decompress))
        {
            gzip.CopyTo(reader);
            var json = Encoding.UTF8.GetString(reader.ToArray());
            return Utils.FromJson<T>(json);
        }
    }

    private static byte[] FormatData(object data)
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