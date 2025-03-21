namespace Known.Data;

/// <summary>
/// 数据模型配置信息类，适用于EFCore配置模型。
/// </summary>
public class DbModelInfo
{
    /// <summary>
    /// 构造函数，初始化数据模型配置信息类。
    /// </summary>
    /// <param name="type">实体类型。</param>
    /// <param name="keys">主键列表。</param>
    public DbModelInfo(Type type, List<string> keys)
    {
        Type = type;
        Keys = keys;
        Fields = GetFields(true);
    }

    /// <summary>
    /// 取得实体类型。
    /// </summary>
    public Type Type { get; }

    /// <summary>
    /// 取得主键字段列表。
    /// </summary>
    public List<string> Keys { get; }

    /// <summary>
    /// 取得字段列表。
    /// </summary>
    public List<FieldInfo> Fields { get; }

    /// <summary>
    /// 获取字段列表。
    /// </summary>
    /// <param name="includeBase">是否包含基类字段。</param>
    /// <returns></returns>
    public List<FieldInfo> GetFields(bool includeBase = false)
    {
        var allFields = Type.IsSubclassOf(typeof(EntityBase)) && includeBase ? TypeHelper.GetBaseFields() : [];
        var fields = TypeHelper.GetFields(Type);
        if (fields != null && fields.Count > 0)
            allFields.AddRange(fields);
        return allFields;
    }
}