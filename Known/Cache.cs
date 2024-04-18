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

        if (!cached.TryGetValue(key, out object value))
            return default;

        return (T)value;
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

    public static List<CodeInfo> GetCodes(string category)
    {
        var infos = new List<CodeInfo>();
        if (string.IsNullOrWhiteSpace(category))
            return infos;

        var codes = GetCodes().Where(c => c.Category == category).ToList();
        if (codes == null || codes.Count == 0)
            codes = category.Split(',', ';').Select(d => new CodeInfo(d, d)).ToList();

        if (codes != null && codes.Count > 0)
            infos.AddRange(codes);

        return infos;
    }

    public static string GetCode(string category, string codeOrName)
    {
        if (string.IsNullOrWhiteSpace(codeOrName))
            return string.Empty;

        var codes = GetCodes(category);
        var code = codes.FirstOrDefault(c => c.Code == codeOrName || c.Name == codeOrName);
        return code?.Code;
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

    internal static void AttachEnumCodes(Type type)
    {
        var codes = TypeHelper.GetEnumCodes(type);
        AttachCodes(codes);
    }

    internal static List<CodeInfo> GetCodes()
    {
        var codes = Get<List<CodeInfo>>(KeyCodes);
        codes ??= [];
        return codes;
    }
}

public class CodeInfo
{
    public CodeInfo(string code, object data = null) : this(code, code, data) { }
    public CodeInfo(string code, string name, object data = null) : this("", code, name, data) { }

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
    public object Data { get; set; }

    public T DataAs<T>()
    {
        if (Data == null)
            return default;

        if (Data is T data)
            return data;

        var dataString = Data.ToString();
        return Utils.FromJson<T>(dataString);
    }
}