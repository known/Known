namespace Known;

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
    /// 取得或设置是否是活动项。
    /// </summary>
    public bool IsActive { get; set; }

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