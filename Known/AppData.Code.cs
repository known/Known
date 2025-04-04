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
    /// 取得或设置实体对应的页面URL。
    /// </summary>
    public string PageUrl { get; set; }

    /// <summary>
    /// 取得或设置代码模型功能集合。
    /// </summary>
    public string[] Functions { get; set; }

    /// <summary>
    /// 取得或设置代码模型字段信息列表
    /// </summary>
    public List<CodeFieldInfo> Fields { get; set; } = [];

    internal virtual string ModelName => $"{Code}Info";
    internal virtual string ModelPath => $"{Namespace}/Models/{ModelName}.cs";
    internal virtual string EntityName => $"{Prefix}{Code}";
    internal virtual string EntityPath => $"{Namespace}.Web/Entities/{EntityName}.cs";
    internal virtual string PageName => $"{Code}List";
    internal virtual string PagePath => $"{Namespace}/Pages/{PageName}.cs";
    internal virtual string FormName => $"{Code}Form";
    internal virtual string FormPath => $"{Namespace}/Pages/{FormName}.razor";
    internal virtual string ServiceName => $"{Code}Service";
    internal virtual string ServiceIPath => $"{Namespace}/Services/{ServiceName}.cs";
    internal virtual string ServicePath => $"{Namespace}.Web/Services/{ServiceName}.cs";

    internal EntityInfo ToEntity()
    {
        var info = new EntityInfo
        {
            Id = Code,
            Name = Name,
            PageUrl = PageUrl,
            Namespace = Namespace,
            ModelName = ModelName,
            EntityName = EntityName,
            PageName = PageName,
            FormName = FormName,
            ServiceName = ServiceName,
            Fields = [.. Fields.Select(f => f.ToField())]
        };
        return info;
    }

    internal PageInfo ToPage()
    {
        var info = new PageInfo();
        info.Tools = Functions?.Where(f => f != "Edit" && f != "Delete").ToList();
        info.Actions = Functions?.Where(f => f == "Edit" || f == "Delete").ToList();
        return info;
    }

    internal FormInfo ToForm()
    {
        var info = new FormInfo();
        info.Fields = [.. Fields.Where(f => f.IsForm).Select(f => f.ToFormField())];
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

    internal FormFieldInfo ToFormField()
    {
        return new FormFieldInfo
        {
            Id = Id,
            Name = Name,
            Type = Type,
            Length = Length,
            Required = Required,
            IsKey = IsKey
        };
    }
}