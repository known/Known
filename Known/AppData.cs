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
            LoadDefaultData();
            return;
        }

        var bytes = File.ReadAllBytes(KmdPath);
        if (OnParseData != null)
            Data = OnParseData(bytes);
        else
            Data = ParseData<AppDataInfo>(bytes);

        // 加载新增菜单页面
        LoadDefaultData();
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

    #region LoadDefaultData
    private static void LoadDefaultData()
    {
        // 加载顶部导航
        if (Data.TopNavs.Count == 0)
            Data.TopNavs = LoadTopNavs();
        // 加载配置的一级模块
        LoadModules();
        // 加载Menu特性的组件菜单
        LoadMenus();
    }

    private static List<PluginInfo> LoadTopNavs()
    {
        return Config.Plugins.Where(p => p.IsNavComponent)
                             .OrderBy(p => p.Attribute.Sort)
                             .Select(p => new PluginInfo { Id = p.Id, Type = p.Id })
                             .ToList();
    }

    private static void LoadModules()
    {
        foreach (var item in Config.Modules)
        {
            if (!Data.Modules.Exists(m => m.Id == item.Id))
                Data.Modules.Add(item);
        }
    }

    private static void LoadMenus()
    {
        foreach (var item in Config.Menus)
        {
            if (Data.Modules.Exists(m => m.Id == item.Page.Name))
                continue;

            var info = new ModuleInfo
            {
                Id = item.Page.Name,
                Name = item.Name,
                Icon = item.Icon,
                ParentId = item.Parent,
                Sort = item.Sort,
                Url = item.Url
            };
            if (TypeHelper.IsSubclassOfGeneric(item.Page, typeof(BaseTablePage<>), out var types))
                info.Plugins.AddPlugin(CreateTablePage(item.Page, types[0]));
            Data.Modules.Add(info);
        }
    }

    private static TablePageInfo CreateTablePage(Type pageType, Type entityType)
    {
        var info = new TablePageInfo
        {
            Page = new PageInfo { Type = pageType.FullName, ShowPager = true, PageSize = Config.App.DefaultPageSize },
            Form = new FormInfo()
        };
        SetMethods(info, pageType);
        SetProperties(info, entityType);
        return info;
    }

    private static void SetMethods(TablePageInfo info, Type pageType)
    {
        var methods = pageType.GetMethods();
        foreach (var item in methods)
        {
            if (item.GetCustomAttribute<ActionAttribute>() is not null)
            {
                if (item.GetParameters().Length == 0)
                    info.Page.Tools.Add(item.Name);
                else
                    info.Page.Actions.Add(item.Name);
            }
        }
    }

    private static void SetProperties(TablePageInfo info, Type entityType)
    {
        var properties = TypeHelper.Properties(entityType);
        foreach (var item in properties)
        {
            var column = item.GetCustomAttribute<ColumnAttribute>();
            if (column != null)
                SetPageColumns(info, item, column);

            var form = item.GetCustomAttribute<FormAttribute>();
            if (form != null)
                SetFormFields(info, item, form);
        }
    }

    private static void SetPageColumns(TablePageInfo info, PropertyInfo item, ColumnAttribute column)
    {
        info.Page.Columns.Add(new PageColumnInfo
        {
            Id = item.Name,
            Name = item.DisplayName(),
            Category = item.Category(),
            Width = item.GetColumnWidth(),
            IsSum = column.IsSum,
            IsSort = column.IsSort,
            DefaultSort = column.DefaultSort,
            IsViewLink = column.IsViewLink,
            IsQuery = column.IsQuery,
            IsQueryAll = column.IsQueryAll,
            Type = column.Type,
            Fixed = column.Fixed,
            Align = column.Align
        });
    }

    private static void SetFormFields(TablePageInfo info, PropertyInfo item, FormAttribute form)
    {
        info.Form.Fields.Add(new FormFieldInfo
        {
            Id = item.Name,
            Name = item.DisplayName(),
            Category = item.Category(),
            Required = item.IsRequired(),
            Row = form.Row,
            Column = form.Column,
            Type = Utils.ConvertTo<FieldType>(form.Type),
            CustomField = form.CustomField,
            ReadOnly = form.ReadOnly,
            Placeholder = form.Placeholder
        });
    }
    #endregion

    #region BizData
    private static Dictionary<string, string> BizData { get; set; } = [];

    internal static T GetBizData<T>(string key)
    {
        if (BizData.TryGetValue(key, out var value))
            return Utils.FromJson<T>(value);

        return default;
    }

    internal static void LoadBizData()
    {
        if (!File.Exists(KdbPath))
            return;

        var bytes = File.ReadAllBytes(KdbPath);
        BizData = ParseData<Dictionary<string, string>>(bytes);
    }

    internal static void SaveBizData(string key, object value)
    {
        BizData[key] = Utils.ToJson(value);
        var bytes = FormatData(BizData);
        File.WriteAllBytes(KdbPath, bytes);
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