namespace Known;

/// <summary>
/// 在线实体模型配置信息类。
/// </summary>
public class EntityInfo
{
    /// <summary>
    /// 取得或设置实体ID。
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 取得或设置实体名称。
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置实体对应的页面URL。
    /// </summary>
    public string PageUrl { get; set; }

    /// <summary>
    /// 取得或设置数据库表名。
    /// </summary>
    public string TableName { get; set; }

    /// <summary>
    /// 取得或设置是否继承实体基类，默认是。
    /// </summary>
    public bool IsEntity { get; set; } = true;

    /// <summary>
    /// 取得或设置是否是工作流实体。
    /// </summary>
    public bool IsFlow { get; set; }

    /// <summary>
    /// 取得或设置实体字段信息列表。
    /// </summary>
    public List<FieldInfo> Fields { get; set; } = [];

    /// <summary>
    /// 取得或设置实体字段代码生成配置信息列表。
    /// </summary>
    [JsonIgnore] public List<CodeFieldInfo> CodeFields { get; set; } = [];
}

/// <summary>
/// 代码生成模型字段信息类。
/// </summary>
public class CodeFieldInfo : FieldInfo
{
    /// <summary>
    /// 取得或设置列表栏位信息。
    /// </summary>
    public PageColumnInfo Column { get; set; }

    /// <summary>
    /// 取得或设置表单栏位信息。
    /// </summary>
    public FormFieldInfo Field { get; set; }

    internal PageColumnInfo ToPageColumn()
    {
        Column ??= new PageColumnInfo
        {
            Id = Id,
            Name = Name,
            Type = Type,
            Length = Length,
            Required = Required
        };
        return Column;
    }

    internal FormFieldInfo ToFormField()
    {
        Field ??= new FormFieldInfo
        {
            Id = Id,
            Name = Name,
            Type = Type,
            Length = Length,
            Required = Required,
            IsKey = IsKey
        };
        return Field;
    }

    /// <summary>
    /// 根据字段信息取得代码生成模型字段信息。
    /// </summary>
    /// <param name="info">字段信息。</param>
    /// <returns></returns>
    public static CodeFieldInfo FromField(FieldInfo info)
    {
        return new CodeFieldInfo
        {
            Id = info.Id,
            Name = info.Name,
            Type = info.Type,
            Length = info.Length,
            Required = info.Required,
            IsKey = info.IsKey
        };
    }
}