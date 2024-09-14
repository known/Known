namespace Known;

/// <summary>
/// 多语言类。
/// </summary>
public class Language
{
    private static readonly ConcurrentDictionary<string, Dictionary<string, object>> caches = new();

    internal Language(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            name = CultureInfo.CurrentCulture.Name;

        Name = name;
    }

    internal string Name { get; }

    /// <summary>
    /// 根据ID取得当前语言字符串。
    /// </summary>
    /// <param name="id">语言标识ID。</param>
    /// <returns></returns>
    public string this[string id]
    {
        get
        {
            var text = GetString(id);
            if (string.IsNullOrEmpty(text))
                return id;

            return text;
        }
    }

    private static List<ActionInfo> items;

    /// <summary>
    /// 取得多语言项目列表（简体中文/繁体中文/English/Việt Nam）。
    /// </summary>
    public static List<ActionInfo> Items
    {
        get
        {
            items ??= [
                new ActionInfo { Id = "zh-CN", Name = "简体中文", Icon = "简" },
                new ActionInfo { Id = "zh-TW", Name = "繁体中文", Icon = "繁" },
                new ActionInfo { Id = "en-US", Name = "English", Icon = "EN" },
                new ActionInfo { Id = "vi-VN", Name = "Việt Nam", Icon = "VN" }
            ];
            return items;
        }
    }

    /// <summary>
    /// 取得首页。
    /// </summary>
    public string Home => this["Menu.Home"];

    /// <summary>
    /// 选择一条数据。
    /// </summary>
    public string SelectOne => this["Tip.SelectOne"];

    /// <summary>
    /// 至少选择一条数据。
    /// </summary>
    public string SelectOneAtLeast => this["Tip.SelectOneAtLeast"];

    /// <summary>
    /// 确定。
    /// </summary>
    public string OK => this["Button.OK"];

    /// <summary>
    /// 取消。
    /// </summary>
    public string Cancel => this["Button.Cancel"];

    /// <summary>
    /// 保存继续。
    /// </summary>
    public string SaveContinue => this["Button.SaveContinue"];

    /// <summary>
    /// 保存关闭。
    /// </summary>
    public string SaveClose => this["Button.SaveClose"];

    /// <summary>
    /// 关闭。
    /// </summary>
    public string Close => this["Button.Close"];

    /// <summary>
    /// 新增。
    /// </summary>
    public string New => this["Button.New"];

    /// <summary>
    /// 编辑。
    /// </summary>
    public string Edit => this["Button.Edit"];

    /// <summary>
    /// 删除。
    /// </summary>
    public string Delete => this["Button.Delete"];

    /// <summary>
    /// 保存。
    /// </summary>
    public string Save => this["Button.Save"];

    /// <summary>
    /// 搜索。
    /// </summary>
    public string Search => this["Button.Search"];

    /// <summary>
    /// 高级搜索。
    /// </summary>
    public string AdvSearch => this["Button.AdvSearch"];

    /// <summary>
    /// 重置。
    /// </summary>
    public string Reset => this["Button.Reset"];

    /// <summary>
    /// 启用。
    /// </summary>
    public string Enable => this["Button.Enable"];

    /// <summary>
    /// 禁用。
    /// </summary>
    public string Disable => this["Button.Disable"];

    /// <summary>
    /// 导入。
    /// </summary>
    public string Import => this["Button.Import"];

    /// <summary>
    /// 导出。
    /// </summary>
    public string Export => this["Button.Export"];

    /// <summary>
    /// 上传。
    /// </summary>
    public string Upload => this["Button.Upload"];

    /// <summary>
    /// 下载。
    /// </summary>
    public string Download => this["Button.Download"];

    /// <summary>
    /// 复制。
    /// </summary>
    public string Copy => this["Button.Copy"];

    /// <summary>
    /// 提交。
    /// </summary>
    public string Submit => this["Button.Submit"];

    /// <summary>
    /// 撤回。
    /// </summary>
    public string Revoke => this["Button.Revoke"];

    /// <summary>
    /// 授权。
    /// </summary>
    public string Authorize => this["Button.Authorize"];

    /// <summary>
    /// 根据语言标识获取语言项目。
    /// </summary>
    /// <param name="name">语言标识</param>
    /// <returns>语言项目对象。</returns>
    public static ActionInfo GetLanguage(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            name = CultureInfo.CurrentCulture.Name;

        var info = Items?.FirstOrDefault(l => l.Id == name);
        info ??= Items?.FirstOrDefault();
        return info;
    }

    internal static void Initialize(Assembly assembly)
    {
        foreach (var item in Items)
        {
            var content = Utils.GetResource(assembly, $"Locales.{item.Id}");
            if (string.IsNullOrWhiteSpace(content))
                continue;

            if (!caches.ContainsKey(item.Id))
                caches[item.Id] = [];
            
            var langs = Utils.FromJson<Dictionary<string, object>>(content);
            if (langs != null && langs.Count > 0)
            {
                foreach (var lang in langs)
                {
                    caches[item.Id][lang.Key] = lang.Value;
                }
            }
        }
    }

    /// <summary>
    /// 根据ID获取语言。
    /// </summary>
    /// <param name="id">ID。</param>
    /// <returns>语言字符串。</returns>
    public string GetString(string id)
    {
        if (string.IsNullOrEmpty(id))
            return "";

        if (!caches.TryGetValue(Name, out Dictionary<string, object> langs))
            return "";

        if (langs == null || !langs.TryGetValue(id, out object value))
            return "";

        return value?.ToString();
    }

    /// <summary>
    /// 获取菜单语言。
    /// </summary>
    /// <param name="info">菜单信息对象。</param>
    /// <returns>菜单语言。</returns>
    public string GetString(MenuInfo info) => GetText("Menu", info?.Code, info?.Name);

    /// <summary>
    /// 获取代码表语言。
    /// </summary>
    /// <param name="info">代码表对象。</param>
    /// <returns>代码表语言。</returns>
    public string GetString(CodeInfo info) => GetText("Code", info?.Code, info?.Name);

    /// <summary>
    /// 获取操作按钮语言。
    /// </summary>
    /// <param name="info">操作按钮对象。</param>
    /// <returns>操作按钮语言。</returns>
    public string GetString(ActionInfo info) => GetText("Button", info?.Id, info?.Name);

    /// <summary>
    /// 获取字段语言。
    /// </summary>
    /// <param name="info">字段信息对象。</param>
    /// <returns>字段语言。</returns>
    public string GetString(ColumnInfo info) => GetText("", info?.Id, info?.Name);

    /// <summary>
    /// 获取指定类型的字段语言。
    /// </summary>
    /// <typeparam name="T">数据类型。</typeparam>
    /// <param name="info">字段信息对象。</param>
    /// <returns>字段语言。</returns>
    public string GetString<T>(ColumnInfo info) => GetText(typeof(T).Name, info?.Id, info?.Name);

    /// <summary>
    /// 获取替换{label}的验证信息的语言。
    /// </summary>
    /// <param name="id">语言ID。</param>
    /// <param name="label">替换的字段名。</param>
    /// <returns>验证信息。</returns>
    public string GetString(string id, string label) => this[id].Replace("{label}", this[label]);

    /// <summary>
    /// 获取替换{label}和{length}的验证信息的语言。
    /// </summary>
    /// <param name="id">语言ID。</param>
    /// <param name="label">替换的字段名。</param>
    /// <param name="length">替换的长度。</param>
    /// <returns>验证信息。</returns>
    public string GetString(string id, string label, int? length) => GetString(id, label).Replace("{length}", $"{length}");

    /// <summary>
    /// 获取替换{label}和{format}的验证信息的语言。
    /// </summary>
    /// <param name="id">语言ID。</param>
    /// <param name="label">替换的字段名。</param>
    /// <param name="format">替换的格式。</param>
    /// <returns>验证信息。</returns>
    public string GetString(string id, string label, string format) => GetString(id, label).Replace("{format}", format);

    /// <summary>
    /// 获取标题语言。
    /// </summary>
    /// <param name="title">标题ID。</param>
    /// <returns>标题语言。</returns>
    public string GetTitle(string title) => GetText("Title", title);

    /// <summary>
    /// 获取代码表语言。
    /// </summary>
    /// <param name="code">代码。</param>
    /// <returns>代码表语言。</returns>
    public string GetCode(string code) => GetText("Code", code);

    /// <summary>
    /// 获取必填验证信息语言。
    /// </summary>
    /// <param name="label">替换的字段名。</param>
    /// <returns>必填验证信息。</returns>
    public string Required(string label) => GetString("Valid.Required", label);

    /// <summary>
    /// 获取操作成功提示语言。
    /// </summary>
    /// <param name="action">操作动作名。</param>
    /// <returns>操作成功提示语言。</returns>
    public string Success(string action) => this["Tip.XXSuccess"].Replace("{action}", action);

    /// <summary>
    /// 获取操作失败提示语言。
    /// </summary>
    /// <param name="action">操作动作名。</param>
    /// <returns>操作失败提示语言。</returns>
    public string Failed(string action) => this["Tip.XXFailed"].Replace("{action}", action);

    /// <summary>
    /// 获取表单的标题语言。
    /// </summary>
    /// <param name="action">表单操作名（新增/编辑）。</param>
    /// <param name="title">表单标题。</param>
    /// <returns>表单的标题语言。</returns>
    public string GetFormTitle(string action, string title)
    {
        var actionName = this[$"Button.{action}"];
        return this["Title.FormAction"]?.Replace("{action}", actionName).Replace("{title}", title);
    }

    /// <summary>
    /// 获取导入表单的标题语言。
    /// </summary>
    /// <param name="name">模块名称。</param>
    /// <returns>导入表单的标题语言。</returns>
    public string GetImportTitle(string name) => this["Title.Import"].Replace("{name}", name);

    internal string GetText(string prefix, string code, string name = null)
    {
        var text = GetString($"{prefix}.{code}");
        if (string.IsNullOrWhiteSpace(text))
            text = GetString(code);
        if (string.IsNullOrWhiteSpace(text))
            text = GetString($"Flow.{code}");
        if (string.IsNullOrWhiteSpace(text))
            text = name;
        if (string.IsNullOrWhiteSpace(text))
            text = code;
        return text;
    }
}