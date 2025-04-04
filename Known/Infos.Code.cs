namespace Known;

/// <summary>
/// 代码生成模型信息类。
/// </summary>
public class CodeModelInfo
{
    /// <summary>
    /// 取得或设置代码模型ID。
    /// </summary>
    public string Id { get; set; } = Utils.GetNextId();

    /// <summary>
    /// 取得或设置代码模型编码。
    /// </summary>
    public string Code { get; set; }

    /// <summary>
    /// 取得或设置代码模型名称。
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置代码模型表前缀。
    /// </summary>
    public string Prefix { get; set; }

    /// <summary>
    /// 取得或设置代码模型命名空间。
    /// </summary>
    public string Namespace { get; set; }

    /// <summary>
    /// 取得或设置代码模型功能集合。
    /// </summary>
    public string[] Functions { get; set; }

    /// <summary>
    /// 取得或设置代码模型字段信息列表
    /// </summary>
    public List<CodeFieldInfo> Fields { get; set; } = [];

    internal EntityInfo ToEntity()
    {
        var info = new EntityInfo
        {
            Id = Code,
            Name = Name,
            Fields = [.. Fields.Select(f => f.ToField())]
        };
        return info;
    }

    internal PageInfo ToPage()
    {
        var info = new PageInfo();
        return info;
    }

    internal FormInfo ToForm()
    {
        var info = new FormInfo();
        return info;
    }
}

/// <summary>
/// 代码生成模型字段信息类。
/// </summary>
public class CodeFieldInfo : FieldInfo
{
    /// <summary>
    /// 取得或设置列表栏位宽度。
    /// </summary>
    public string Width { get; set; }

    /// <summary>
    /// 取得或设置字段是否主键。
    /// </summary>
    public bool IsGrid { get; set; }

    /// <summary>
    /// 取得或设置字段是否主键。
    /// </summary>
    public bool IsForm { get; set; }
}