using System.Collections.Concurrent;
using System.Globalization;
using Known.Entities;

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

    public static List<ActionInfo> Items { get; } = [];
    public string Home => this["Menu.Home"];

    internal string SelectOne => this["Tip.SelectOne"];
    public string SelectOneAtLeast => this["Tip.SelectOneAtLeast"];

    public string OK => this["Button.OK"];
    public string Cancel => this["Button.Cancel"];
    internal string New => this["Button.New"];
    public string Edit => this["Button.Edit"];
    public string Delete => this["Button.Delete"];
    public string Save => this["Button.Save"];
    public string Search => this["Button.Search"];
    public string AdvSearch => this["Button.AdvSearch"];
    public string Reset => this["Button.Reset"];
    internal string Enable => this["Button.Enable"];
    internal string Disable => this["Button.Disable"];
    public string Import => this["Button.Import"];
    internal string Export => this["Button.Export"];
    internal string Upload => this["Button.Upload"];
    internal string Download => this["Button.Download"];
    internal string Copy => this["Button.Copy"];
    internal string Submit => this["Button.Submit"];
    internal string Revoke => this["Button.Revoke"];
    internal string Authorize => this["Button.Authorize"];

    public static ActionInfo GetLanguage(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            name = CultureInfo.CurrentCulture.Name;

        var info = Items?.FirstOrDefault(l => l.Id == name);
        info ??= Items?.FirstOrDefault();
        return info;
    }

    internal static void Initialize()
    {
        var path = Path.GetFullPath("Locales");
        if (!Directory.Exists(path))
            return;

        var files = Directory.GetFiles(path);
        foreach (var file in files)
        {
            var name = new FileInfo(file).Name.Split('.')[0];
            var json = File.ReadAllText(file);
            var lang = Utils.FromJson<Dictionary<string, object>>(json);
            caches[name] = lang;
            Items.Add(new ActionInfo
            {
                Id = name,
                Name = lang["localeName"].ToString(),
                Icon = lang["localeIcon"].ToString()
            });
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

    internal string GetString(SysModule module) => GetText("Menu", module?.Code, module?.Name);
    internal string GetString(MenuInfo info) => GetText("Menu", info.Code, info.Name);
    public string GetString(MenuItem item) => GetText("Menu", item.Code, item.Name);
    public string GetString(CodeInfo info) => GetText("Code", info.Code, info.Name);
    public string GetString(ActionInfo info) => GetText("Button", info.Id, info.Name);
    public string GetString(ColumnInfo info) => GetText("", info.Id, info.Name);
    public string GetString<T>(ColumnInfo info) => GetText(typeof(T).Name, info.Id, info.Name);
    public string GetString(string id, string label) => this[id].Replace("{label}", this[label]);
    public string GetString(string id, string label, int? length) => GetString(id, label).Replace("{length}", $"{length}");
    internal string GetString(string id, string label, string format) => GetString(id, label).Replace("{format}", format);

    public string GetTitle(string title) => GetText("Title", title);
    public string GetCode(string code) => GetText("Code", code);
    public string Required(string label) => GetString("Valid.Required", label);
    internal string Success(string action) => this["Tip.XXSuccess"].Replace("{action}", action);
    internal string Failed(string action) => this["Tip.XXFailed"].Replace("{action}", action);

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