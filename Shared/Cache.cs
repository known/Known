namespace Known;

/// <summary>
/// 框架内存缓存类。
/// </summary>
public sealed class Cache
{
    private static readonly string KeyCodes = $"Known_Codes_{Config.App.Id}";
    private static readonly CacheService<object> cache = new();
    private static readonly CacheService<UserInfo> user = new();

    private Cache() { }

    /// <summary>
    /// 根据Key获取缓存泛型对象。
    /// </summary>
    /// <typeparam name="T">对象泛型类型。</typeparam>
    /// <param name="key">缓存键。</param>
    /// <returns>缓存泛型对象。</returns>
    public static T Get<T>(string key)
    {
        var value = cache.Get(key);
        if (value == null)
            return default;

        return (T)value;
    }

    /// <summary>
    /// 设置缓存对象。
    /// </summary>
    /// <param name="key">缓存键。</param>
    /// <param name="value">缓存对象。</param>
    /// <param name="timeSpan">缓存过期时间。</param>
    public static void Set(string key, object value, TimeSpan? timeSpan = null)
    {
        cache.Set(key, value, timeSpan);
    }

    /// <summary>
    /// 移除缓存对象。
    /// </summary>
    /// <param name="key">缓存键。</param>
    public static void Remove(string key)
    {
        cache.Remove(key);
    }

    /// <summary>
    /// 获取缓存登录用户信息。
    /// </summary>
    /// <param name="userName">用户名。</param>
    /// <returns>用户信息。</returns>
    public static UserInfo GetUser(string userName)
    {
        return user.Get(userName);
    }

    /// <summary>
    /// 根据Token获取缓存的登录用户信息。
    /// </summary>
    /// <param name="token">Token。</param>
    /// <returns>用户信息。</returns>
    public static UserInfo GetUserByToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return null;

        return user.Values.FirstOrDefault(x => x.Token == token);
    }

    /// <summary>
    /// 将登录用户添加到缓存里，Key为用户名。
    /// </summary>
    /// <param name="info">用户信息。</param>
    /// <param name="timeSpan">用户缓存过期时间。</param>
    public static void SetUser(UserInfo info, TimeSpan? timeSpan = null)
    {
        if (info == null)
            return;

        if (timeSpan == null)
            timeSpan = Config.App.AuthExpired;

        user.Set(info.UserName, info, timeSpan);
    }

    /// <summary>
    /// 将登录用户从缓存里移除。
    /// </summary>
    /// <param name="info">用户信息。</param>
    public static void RemoveUser(UserInfo info)
    {
        if (info == null)
            return;

        user.Remove(info.UserName);
    }

    /// <summary>
    /// 获取枚举类型的代码表列表。
    /// </summary>
    /// <typeparam name="T">枚举类型。</typeparam>
    /// <returns>代码表列表。</returns>
    public static List<CodeInfo> GetCodes<T>() where T : Enum
    {
        return GetCodes(typeof(T).Name);
    }

    /// <summary>
    /// 根据代码类别名获取代码表列表，或由可数项目转换成代码表（用逗号分割，如：项目1,项目2）。
    /// </summary>
    /// <param name="category">代码类别名。</param>
    /// <returns>代码表列表。</returns>
    public static List<CodeInfo> GetCodes(string category) => GetCodes(category, null);

    /// <summary>
    /// 根据代码类别名获取代码表列表，或由可数项目转换成代码表（用逗号分割，如：项目1,项目2）。
    /// </summary>
    /// <param name="category">代码类别名。</param>
    /// <param name="nameFormat">代码名称显示格式，比如：{Code}-{Name}，默认只显示名称。</param>
    /// <returns>代码表列表。</returns>
    public static List<CodeInfo> GetCodes(string category, string nameFormat)
    {
        var infos = new List<CodeInfo>();
        if (string.IsNullOrWhiteSpace(category))
            return infos;

        var codes = GetCodes().Where(c => c.Category == category).ToList();
        if (codes == null || codes.Count == 0)
            codes = [.. category.Split(',', ';', '，', '；').Select(d => new CodeInfo(d, d))];

        foreach (var item in codes)
        {
            var code = new CodeInfo(item.Category, item.Code, item.Name);
            if (code.Code != code.Name && !string.IsNullOrWhiteSpace(nameFormat))
                code.Name = nameFormat.Replace("{Code}", item.Code).Replace("{Name}", item.Name);
            infos.Add(code);
        }
        return infos;
    }

    /// <summary>
    /// 根据代码类别名和代码（或名称）获取代码表中项目的编码。
    /// </summary>
    /// <param name="category">代码类别名。</param>
    /// <param name="codeOrName">代码（或名称）。</param>
    /// <returns>项目的编码。</returns>
    public static string GetCode(string category, string codeOrName)
    {
        if (string.IsNullOrWhiteSpace(codeOrName))
            return string.Empty;

        var codes = GetCodes(category);
        var code = codes.FirstOrDefault(c => c.Code == codeOrName || c.Name == codeOrName);
        return code?.Code ?? codeOrName;
    }

    /// <summary>
    /// 根据代码类别名和代码（或名称）获取代码表中项目的名称。
    /// </summary>
    /// <param name="category">代码类别名。</param>
    /// <param name="codeOrName">代码（或名称）。</param>
    /// <returns>项目的名称。</returns>
    public static string GetCodeName(string category, string codeOrName)
    {
        if (string.IsNullOrWhiteSpace(codeOrName))
            return string.Empty;

        var codes = GetCodes(category);
        var code = codes.FirstOrDefault(c => c.Code == codeOrName || c.Name == codeOrName);
        return code?.Name ?? code?.Code ?? codeOrName;
    }

    /// <summary>
    /// 根据代码类别名和代码（或名称）获取代码表中项目的名称。
    /// </summary>
    /// <param name="category">代码类别名。</param>
    /// <param name="codeOrNames">代码（或名称）集合。</param>
    /// <returns>项目的名称。</returns>
    public static string GetCodeName(string category, string[] codeOrNames)
    {
        if (codeOrNames == null || codeOrNames.Length == 0)
            return string.Empty;

        var codes = GetCodes(category);
        var names = new List<string>();
        foreach (var item in codeOrNames)
        {
            var code = codes.FirstOrDefault(c => c.Code == item || c.Name == item);
            var name = code?.Name ?? code?.Code ?? item;
            names.Add(name);
        }
        return string.Join(", ", names);
    }

    /// <summary>
    /// 附加代码表对象列表到框架缓存中。
    /// </summary>
    /// <param name="codes">代码表对象列表。</param>
    public static void AttachCodes(List<CodeInfo> codes)
    {
        if (codes == null || codes.Count == 0)
            return;

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