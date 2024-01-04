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
            if (string.IsNullOrEmpty(id))
                return "";

            if (!caches.TryGetValue(lang, out Dictionary<string, object> langs))
                return id;

            if (langs == null || !langs.TryGetValue(id, out object value))
                return id;

            return value?.ToString();
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

    public string Success(string action) => this["Tip.XXSuccess"].Replace("{action}", action);
    public string GetString(string id, string label) => this[id].Replace("{label}", label);
    public string GetString(string id, string label, int? length) => GetString(id, label).Replace("{length}", $"{length}");
    public string GetString(string id, string label, string format) => GetString(id, label).Replace("{format}", format);

    public string Home => this["Menu.Home"];

    public string SelectOne => this["Tip.SelectOne"];
    public string SelectOneAtLeast => this["Tip.SelectOneAtLeast"];
    public string BasicInfo => this["Title.BasicInfo"];

    public string OK => this["Button.OK"];
    public string Cancel => this["Button.Cancel"];
    public string New => this["Button.New"];
    public string Edit => this["Button.Edit"];
    public string Delete => this["Button.Delete"];
    public string Save => this["Button.Save"];
    public string Search => this["Button.Search"];
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
}