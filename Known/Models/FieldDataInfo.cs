namespace Known.Models;

/// <summary>
/// 字段配置数据信息类。
/// </summary>
public class FieldDataInfo
{
    /// <summary>
    /// 取得或设置字段ID。
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 取得或设置字段代码。
    /// </summary>
    [Required]
    [Column, Form]
    [DisplayName("代码")]
    public string Code { get; set; }

    /// <summary>
    /// 取得或设置字段名称。
    /// </summary>
    [Required]
    [Column, Form]
    [DisplayName("名称")]
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置字段类型。
    /// </summary>
    [Required]
    [Column]
    [Form(Type = nameof(FieldType.Select))]
    [Category(nameof(FieldType))]
    [DisplayName("类型")]
    public string Type { get; set; }

    /// <summary>
    /// 取得或设置字段长度。
    /// </summary>
    [Required]
    [Column, Form]
    [DisplayName("长度")]
    public string Length { get; set; }

    /// <summary>
    /// 转换成字段信息对象。
    /// </summary>
    /// <returns></returns>
    public FieldInfo ToField()
    {
        return new FieldInfo
        {
            Id = Code,
            Name = Name,
            TypeName = Type,
            Length = Length
        };
    }

    internal PageColumnInfo ToColumn()
    {
        return new PageColumnInfo
        {
            Id = Code,
            Name = Name,
            Type = Utils.ConvertTo<FieldType>(Type),
            Length = Length
        };
    }
}