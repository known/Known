namespace Known;

/// <summary>
/// 框架内存缓存类。
/// </summary>
public sealed class Cache
{
    private static readonly string KeyCodes = $"Known_Codes_{Config.App.Id}";
    private static readonly ConcurrentDictionary<string, object> cached = new();
    private static readonly ConcurrentDictionary<string, UserInfo> cachedUsers = new();

    private Cache() { }

    /// <summary>
    /// 根据Key获取缓存泛型对象。
    /// </summary>
    /// <typeparam name="T">对象泛型类型。</typeparam>
    /// <param name="key">缓存键。</param>
    /// <returns>缓存泛型对象。</returns>
    public static T Get<T>(string key)
    {
        if (string.IsNullOrEmpty(key))
            return default;

        if (!cached.TryGetValue(key, out object value))
            return default;

        return (T)value;
    }

    /// <summary>
    /// 设置缓存对象。
    /// </summary>
    /// <param name="key">缓存键。</param>
    /// <param name="value">缓存对象。</param>
    public static void Set(string key, object value)
    {
        if (string.IsNullOrEmpty(key))
            return;

        cached[key] = value;
    }

    /// <summary>
    /// 移除缓存对象。
    /// </summary>
    /// <param name="key">缓存键。</param>
    public static void Remove(string key)
    {
        if (string.IsNullOrEmpty(key))
            return;

        if (!cached.ContainsKey(key))
            return;

        cached.TryRemove(key, out object _);
    }

    /// <summary>
    /// 获取缓存登录用户信息。
    /// </summary>
    /// <param name="key">用户名。</param>
    /// <returns>用户信息。</returns>
    public static UserInfo GetUser(string key)
    {
        cachedUsers.TryGetValue(key, out UserInfo user);
        return user;
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

        return cachedUsers.Values.FirstOrDefault(u => u.Token == token);
    }

    /// <summary>
    /// 将登录用户添加到缓存里。
    /// </summary>
    /// <param name="user">用户信息。</param>
    public static void SetUser(UserInfo user)
    {
        if (user == null)
            return;

        cachedUsers[user.UserName] = user;
    }

    /// <summary>
    /// 将登录用户从缓存里移除。
    /// </summary>
    /// <param name="user">用户信息。</param>
    public static void RemoveUser(UserInfo user)
    {
        if (user == null)
            return;

        cachedUsers.TryRemove(user.UserName, out UserInfo _);
    }

    /// <summary>
    /// 根据代码类别名获取代码表列表，或由可数项目转换成代码表（用逗号分割，如：项目1,项目2）。
    /// </summary>
    /// <param name="category">代码类别名/</param>
    /// <returns>代码表列表。</returns>
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
        return code?.Code;
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
        return code?.Name ?? code?.Code;
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

/// <summary>
/// 代码表信息类。
/// </summary>
public class CodeInfo
{
    /// <summary>
    /// 构造函数，创建一个代码表信息类的实例。
    /// </summary>
    public CodeInfo() { }

    /// <summary>
    /// 构造函数，创建一个代码表信息类的实例。
    /// </summary>
    /// <param name="code">编码。</param>
    /// <param name="name">名称。</param>
    /// <param name="data">关联的数据对象，默认null。</param>
    public CodeInfo(string code, string name, object data = null) : this("", code, name, data) { }

    /// <summary>
    /// 构造函数，创建一个代码表信息类的实例。
    /// </summary>
    /// <param name="category">代码类别名。</param>
    /// <param name="code">编码。</param>
    /// <param name="name">名称。</param>
    /// <param name="data">关联的数据对象，默认null。</param>
    public CodeInfo(string category, string code, string name, object data = null)
    {
        Category = category;
        Code = code;
        Name = name;
        Data = data;
    }

    /// <summary>
    /// 取得或设置代码类别。
    /// </summary>
    public string Category { get; set; }

    /// <summary>
    /// 取得或设置编码。
    /// </summary>
    public string Code { get; set; }

    /// <summary>
    /// 取得或设置名称。
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置关联的数据对象。
    /// </summary>
    public object Data { get; set; }

    /// <summary>
    /// 将关联的数据对象转换成泛型对象。
    /// </summary>
    /// <typeparam name="T">泛型对象类型。</typeparam>
    /// <returns>泛型对象。</returns>
    public T DataAs<T>()
    {
        if (Data == null)
            return default;

        if (Data is T data)
            return data;

        var dataString = Data.ToString();
        return Utils.FromJson<T>(dataString);
    }

    /// <summary>
    /// 获取代码表对象的显示字符串，显示名称属性。
    /// </summary>
    /// <returns></returns>
    public override string ToString() => Name;
}