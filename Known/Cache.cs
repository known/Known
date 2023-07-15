namespace Known;

public sealed class Cache
{
    private static readonly string KeyCodes = $"Known_Codes_{Config.AppId}";
    private static readonly ConcurrentDictionary<string, object> cached = new();

    private Cache() { }
    static Cache()
    {
        var assembly = typeof(Cache).Assembly;
        AttachCodes(assembly);
    }

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

    public static List<CodeInfo> GetCodes(string category) => GetCodes().Where(c => c.Category == category).ToList();

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

    public static void AttachCodes(Assembly assembly)
    {
        var types = assembly.GetExportedTypes();
        if (types == null || types.Length == 0)
            return;

        foreach (var item in types)
        {
            var attr = item.GetCustomAttributes<CodeTableAttribute>();
            if (attr != null && attr.Any())
                AttachCodes(item);
        }
    }

    private static void AttachCodes(Type type)
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