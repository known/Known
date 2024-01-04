using System.Collections.Concurrent;
using System.Globalization;

namespace Known;

public class Language
{
    private readonly string lang;
    private static readonly ConcurrentDictionary<string, Dictionary<string, object>> caches = new();

    internal Language(string lang)
    {
        if (string.IsNullOrWhiteSpace(lang))
            lang = CultureInfo.CurrentCulture.Name;

        this.lang = lang;
    }

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

    public string GetColumnName<T>(string id)
    {
        var key = $"{typeof(T).Name}.{id}";
        var name = GetString(key);
        if (string.IsNullOrWhiteSpace(name))
            name = this[id];
        return name;
    }

    internal string GetString(string id)
    {
        if (string.IsNullOrEmpty(id))
            return "";

        if (!caches.TryGetValue(lang, out Dictionary<string, object> langs))
            return "";

        if (langs == null || !langs.TryGetValue(id, out object value))
            return "";

        return value?.ToString();
    }

    public string GetString(string id, string label) => this[id].Replace("{label}", label);
    public string GetString(string id, string label, int? length) => GetString(id, label).Replace("{length}", $"{length}");
    internal string GetString(string id, string label, string format) => GetString(id, label).Replace("{format}", format);
    internal string Success(string action) => this["Tip.XXSuccess"].Replace("{action}", action);

    public string Home => this["Menu.Home"];

    internal string SelectOne => this["Tip.SelectOne"];
    public string SelectOneAtLeast => this["Tip.SelectOneAtLeast"];
    public string BasicInfo => this["Title.BasicInfo"];

    public string OK => this["Button.OK"];
    public string Cancel => this["Button.Cancel"];
    internal string New => this["Button.New"];
    public string Edit => this["Button.Edit"];
    public string Delete => this["Button.Delete"];
    public string Save => this["Button.Save"];
    public string Search => this["Button.Search"];
    public string Reset => this["Button.Reset"];
    internal string Enable => this["Button.Enable"];
    internal string Disable => this["Button.Disable"];
    internal string Import => this["Button.Import"];
    internal string Export => this["Button.Export"];
    internal string Upload => this["Button.Upload"];
    internal string Download => this["Button.Download"];
    internal string Copy => this["Button.Copy"];
    internal string Submit => this["Button.Submit"];
    internal string Revoke => this["Button.Revoke"];
}