namespace Known.Data;

/// <summary>
/// 数据模型配置信息类，适用于EFCore配置模型。
/// </summary>
public class DbModelInfo
{
    /// <summary>
    /// 取得或设置实体类型。
    /// </summary>
    public Type Type { get; set; }

    /// <summary>
    /// 取得或设置主键字段列表。
    /// </summary>
    public List<string> Keys { get; set; }

    internal List<FieldInfo> Fields { get; set; }

    internal void InitFields()
    {
        Fields = Type.IsSubclassOf(typeof(EntityBase)) ? TypeHelper.GetBaseFields() : [];
        var fields = TypeHelper.GetFields(Type);
        if (fields != null && fields.Count > 0)
            Fields.AddRange(fields);
    }
}