using System.Collections.Concurrent;
using System.Reflection;
using Known.Entities;

namespace Known;

public sealed class Cache
{
    private static readonly string KeyCodes = $"Known_Codes_{Config.App.Id}";
    private static readonly ConcurrentDictionary<string, object> cached = new();

    private Cache() { }

    public static T Get<T>(string key)
    {
        if (string.IsNullOrEmpty(key))
            return default;

        if (!cached.ContainsKey(key))
            return default;

        return (T)cached[key];
    }

    public static void Set(string key, object value)
    {
        if (string.IsNullOrEmpty(key))
            return;

        cached[key] = value;
    }

    public static void Remove(string key)
    {
        if (string.IsNullOrEmpty(key))
            return;

        if (!cached.ContainsKey(key))
            return;

        cached.TryRemove(key, out object _);
    }

    public static List<CodeInfo> GetCodes(string category, bool isAll = true)
    {
        var codes = GetCodes().Where(c => c.Category == category).ToList();
        if (isAll)
        {
            codes.Insert(0, new CodeInfo("", "全部"));
        }
        return codes;
    }

    public static void AddDicCategory<T>()
    {
        var datas = new List<CodeInfo>();
        var fields = typeof(T).GetFields();
        foreach (var item in fields)
        {
            if (!item.IsLiteral)
                continue;

            var value = item.GetRawConstantValue()?.ToString();
            datas.Add(new CodeInfo(Constants.DicCategory, value, value));
        }
        AttachCodes(datas);
    }

    public static void AttachCodes(List<CodeInfo> codes)
    {
        var datas = new List<CodeInfo>();
        var items = GetCodes();
        if (items != null && items.Count > 0)
        {
            foreach (var item in items)
            {
                if (!codes.Exists(c => c.Category == item.Category))
                    datas.Add(item);
            }
        }

        datas.AddRange(codes);
        Set(KeyCodes, datas);
    }

    internal static void AttachCodes(Type type)
    {
        var fields = type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        if (fields == null || fields.Length == 0)
            return;

        var datas = new List<CodeInfo>();
        foreach (var item in fields)
        {
            var name = item.GetValue(null).ToString();
            var code = new CodeInfo(type.Name, name, name, null);
            datas.Add(code);
        }
        AttachCodes(datas);
    }

    private static List<CodeInfo> GetCodes()
    {
        var codes = Get<List<CodeInfo>>(KeyCodes);
        codes ??= new List<CodeInfo>();
        return codes;
    }
}

public class CodeInfo
{
    public CodeInfo() { }

    public CodeInfo(string code, string name, object data = null)
    {
        Code = code;
        Name = name;
        Data = data;
    }

    public CodeInfo(string category, string code, string name, object data = null)
    {
        Category = category;
        Code = code;
        Name = name;
        Data = data;
    }

    public string Category { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public bool IsActive { get; set; }
    public object Data { get; set; }

    public T DataAs<T>()
    {
        if (Data == null)
            return default;

        var dataString = Data.ToString();
        return Utils.FromJson<T>(dataString);
    }

    public static List<CodeInfo> GetCodes(string category)
    {
        if (string.IsNullOrEmpty(category))
            return new List<CodeInfo>();

        var codes = Cache.GetCodes(category);
        if (codes != null && codes.Count > 0)
            return codes;

        return category.Split(',', ';').Select(d => new CodeInfo(d, d)).ToList();
    }

    public static CodeInfo[] GetChildCodes(string category, string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        var codes = GetCodes(category);
        var code = codes.FirstOrDefault(c => c.Code == value);
        var dic = code?.Data is SysDictionary
                ? code?.Data as SysDictionary
                : Utils.FromJson<SysDictionary>(code?.Data?.ToString());
        if (!string.IsNullOrWhiteSpace(dic?.Child))
            return dic?.Children.Select(c => new CodeInfo(c.Code, c.Name)).ToArray();

        if (!string.IsNullOrWhiteSpace(dic?.Extension))
        {
            return dic?.Extension.Split(Environment.NewLine.ToArray())
                       .Where(s => !string.IsNullOrWhiteSpace(s))
                       .Select(s => new CodeInfo(s, s))
                       .ToArray();
        }

        return null;
    }

    public static string GetCode(string category, string codeOrName)
    {
        if (string.IsNullOrWhiteSpace(codeOrName))
            return string.Empty;

        var codes = GetCodes(category);
        var code = codes.FirstOrDefault(c => c.Code == codeOrName || c.Name == codeOrName);
        return code?.Code;
    }
}