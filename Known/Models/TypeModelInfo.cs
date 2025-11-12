namespace Known.Models;

/// <summary>
/// 类型模型信息类。
/// </summary>
public class TypeModelInfo
{
    /// <summary>
    /// 构造函数。
    /// </summary>
    /// <param name="properties">类型属性集合。</param>
    public TypeModelInfo(PropertyInfo[] properties)
    {
        Properties = properties;
        Fields = [.. properties.Select(p => new TypeFieldInfo(p))];
        Dictionary = Fields.ToFrozenDictionary(p => p.Name, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// 取得类型属性集合。
    /// </summary>
    public PropertyInfo[] Properties { get; }

    /// <summary>
    /// 取得类型字段信息列表。
    /// </summary>
    public List<TypeFieldInfo> Fields { get; }

    /// <summary>
    /// 取得类型字段字典。
    /// </summary>
    public FrozenDictionary<string, TypeFieldInfo> Dictionary { get; }

    /// <summary>
    /// 根据属性名获取属性信息。
    /// </summary>
    /// <param name="name">属性名。</param>
    /// <returns></returns>
    public PropertyInfo GetProperty(string name) => Dictionary.GetValueOrDefault(name)?.Property;

    /// <summary>
    /// 获取指定对象的属性值。
    /// </summary>
    /// <param name="instance">对象实例。</param>
    /// <param name="name">属性名。</param>
    /// <returns>属性值。</returns>
    public object GetValue(object instance, string name)
    {
        if (Dictionary.TryGetValue(name, out var info))
            return info.GetValue(instance);
        return default;
    }

    /// <summary>
    /// 设置指定对象的属性值。
    /// </summary>
    /// <param name="instance">对象实例。</param>
    /// <param name="name">属性名。</param>
    /// <param name="value">属性值。</param>
    public void SetValue(object instance, string name, object value)
    {
        if (Dictionary.TryGetValue(name, out var info))
            info.SetValue(instance, value);
    }

    internal void SetDBValue(object instance, string key, object value)
    {
        if (!Dictionary.TryGetValue(key, out var info))
            return;

        var property = info.Property;
        var type = property.PropertyType;
        if (type.IsValueType || type == typeof(string))
        {
            info.SetValue(instance, value);
        }
        else
        {
            var data = Utils.FromJson(type, value?.ToString());
            info.SetValue(instance, data);
        }
    }

    internal object GetDBValue(object instance, PropertyInfo item)
    {
        if (instance == null)
            return null;

        var type = item.PropertyType;
        if (type.IsValueType || type == typeof(string))
            return GetValue(instance, item.Name);

        var value = GetValue(instance, item.Name);
        return Utils.ToJson(value);
    }

    internal List<ColumnInfo> GetColumns(bool isAttr)
    {
        var columns = new List<ColumnInfo>();
        foreach (var item in Fields)
        {
            var column = item.GetColumn(isAttr);
            if (column != null)
                columns.Add(column);
        }
        return columns;
    }

    internal Dictionary<string, ColumnInfo> GetFormns()
    {
        var forms = new Dictionary<string, ColumnInfo>();
        foreach (var item in Fields)
        {
            var form = item.GetForm();
            if (form != null)
                forms[item.Name] = form;
        }
        return forms;
    }
}