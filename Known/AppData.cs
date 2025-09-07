namespace Known;

/// <summary>
/// 框架配置数据，从代码中自动加载由特性配置的数据。
/// </summary>
public sealed partial class AppData
{
    private AppData() { }

    // 模块配置文件路径
    internal static string KmdPath { get; set; }

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
    /// 取得或设置自动页面插件配置信息委托。
    /// </summary>
    public static Func<PluginInfo, AutoPageInfo> OnAutoPage { get; set; }

    /// <summary>
    /// 根据ID获取模块信息。
    /// </summary>
    /// <param name="id">模块ID。</param>
    /// <returns>模块信息。</returns>
    internal static ModuleInfo GetModule(string id)
    {
        return Data.Modules?.FirstOrDefault(m => m.Id == id);
    }

    /// <summary>
    /// 获取系统所有按钮信息列表。
    /// </summary>
    /// <returns></returns>
    public static List<ButtonInfo> GetButtons()
    {
        var datas = Data.Buttons ?? [];
        foreach (var item in Config.Actions)
        {
            if (datas.Exists(d => d.Id == item.Id))
                continue;

            datas.Add(item.ToButton());
        }
        return datas;
    }

    /// <summary>
    /// 获取系统所有操作信息列表。
    /// </summary>
    /// <returns></returns>
    public static List<ActionInfo> GetActions()
    {
        var datas = Data.Buttons?.Select(b => b.ToAction()).ToList() ?? [];
        foreach (var item in Config.Actions)
        {
            if (datas.Exists(d => d.Id == item.Id))
                continue;

            datas.Add(item);
        }
        return datas;
    }

    /// <summary>
    /// 创建自动页面插件配置信息。
    /// </summary>
    /// <param name="pageType">页面组件类型。</param>
    /// <returns>插件配置信息。</returns>
    public static AutoPageInfo CreateAutoPage(Type pageType)
    {
        if (pageType.BaseType.IsGenericType)
        {
            var arguments = pageType.BaseType.GetGenericArguments();
            return AppDefaultData.CreateAutoPage(pageType, arguments[0]);
        }

        if (pageType.BaseType.BaseType.IsGenericType)
        {
            var arguments = pageType.BaseType.BaseType.GetGenericArguments();
            return AppDefaultData.CreateAutoPage(pageType, arguments[0]);
        }

        return AppDefaultData.CreateAutoPage(pageType);
    }

    #region AppData
    /// <summary>
    /// 加载默认菜单配置数据。
    /// </summary>
    public static void LoadAppData()
    {
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

    internal static void LoadModules()
    {
        AppDefaultData.LoadModule(Data);
    }
    #endregion

    #region Private
    private static T ParseData<T>(byte[] bytes)
    {
        return ZipHelper.UnZipData<T>(bytes);
    }

    private static byte[] FormatData(object data)
    {
        return ZipHelper.ZipData(data);
    }
    #endregion
}