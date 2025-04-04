namespace Known;

/// <summary>
/// 在线实体字段配置信息类。
/// </summary>
public class FieldInfo
{
    /// <summary>
    /// 取得或设置字段ID。
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 取得或设置字段名称。
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置字段类型。
    /// </summary>
    public FieldType Type { get; set; }

    /// <summary>
    /// 取得或设置字段长度。
    /// </summary>
    public string Length { get; set; }

    /// <summary>
    /// 取得或设置字段是否必填。
    /// </summary>
    public bool Required { get; set; }

    /// <summary>
    /// 取得或设置字段是否主键。
    /// </summary>
    public bool IsKey { get; set; }

    /// <summary>
    /// 取得或设置字段类型名称。
    /// </summary>
    [JsonIgnore]
    public string TypeName
    {
        get { return Type.ToString(); }
        set { Type = Utils.ConvertTo<FieldType>(value); }
    }

    /// <summary>
    /// 取得或设置字段是否是列表栏位。
    /// </summary>
    public bool IsGrid { get; set; }

    /// <summary>
    /// 取得或设置字段是否是表单字段。
    /// </summary>
    public bool IsForm { get; set; }

    /// <summary>
    /// 转换成字段信息。
    /// </summary>
    /// <returns></returns>
    public FieldInfo ToField()
    {
        return new FieldInfo
        {
            Id = Id,
            Name = Name,
            Type = Type,
            Length = Length,
            Required = Required,
            IsKey = IsKey,
            IsGrid = IsGrid,
            IsForm = IsForm
        };
    }
}