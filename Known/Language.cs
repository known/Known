namespace Known;

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

    public string Home => this["Menu.Home"];

    public string SelectOne => this["Tip.SelectOne"];
    public string SelectOneAtLeast => this["Tip.SelectOneAtLeast"];
    public string SysModule => this["Menu.SysModuleList"];

    public string OK => this["Button.OK"];
    public string Cancel => this["Button.Cancel"];
    public string SaveContinue => this["Button.SaveContinue"];
    public string SaveClose => this["Button.SaveClose"];
    public string Close => this["Button.Close"];
    public string New => this["Button.New"];
    public string Edit => this["Button.Edit"];
    public string Delete => this["Button.Delete"];
    public string Save => this["Button.Save"];
    public string Search => this["Button.Search"];
    public string AdvSearch => this["Button.AdvSearch"];
    public string Reset => this["Button.Reset"];
    public string Enable => this["Button.Enable"];
    public string Disable => this["Button.Disable"];
    public string Import => this["Button.Import"];
    public string Export => this["Button.Export"];
    public string Upload => this["Button.Upload"];
    public string Download => this["Button.Download"];
    public string Copy => this["Button.Copy"];
    public string Submit => this["Button.Submit"];
    public string Revoke => this["Button.Revoke"];
    public string Authorize => this["Button.Authorize"];

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

    public string GetString(MenuInfo info) => GetText("Menu", info.Code, info.Name);
    public string GetString(CodeInfo info) => GetText("Code", info.Code, info.Name);
    public string GetString(ActionInfo info) => GetText("Button", info.Id, info.Name);
    public string GetString(ColumnInfo info) => GetText("", info.Id, info.Name);
    public string GetString<T>(ColumnInfo info) => GetText(typeof(T).Name, info.Id, info.Name);
    public string GetString(string id, string label) => this[id].Replace("{label}", this[label]);
    public string GetString(string id, string label, int? length) => GetString(id, label).Replace("{length}", $"{length}");
    public string GetString(string id, string label, string format) => GetString(id, label).Replace("{format}", format);
    public string GetImportTitle(string name) => this["Title.Import"].Replace("{name}", name);
    public string GetTitle(string title) => GetText("Title", title);
    public string GetCode(string code) => GetText("Code", code);
    public string Required(string label) => GetString("Valid.Required", label);
    public string Success(string action) => this["Tip.XXSuccess"].Replace("{action}", action);
    public string Failed(string action) => this["Tip.XXFailed"].Replace("{action}", action);

    public string GetFormTitle(string action, string title)
    {
        var actionName = this[$"Button.{action}"];
        return this["Title.FormAction"]?.Replace("{action}", actionName).Replace("{title}", title);
    }

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