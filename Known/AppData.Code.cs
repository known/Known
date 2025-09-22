namespace Known;

/// <summary>
/// 代码生成模型信息类。
/// </summary>
public class CodeModelInfo
{
    internal bool IsNew { get; set; }

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
    public string Namespace { get; set; } = Config.App.Id;

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

    /// <summary>
    /// 取得或设置是否是Auto模式。
    /// </summary>
    [JsonIgnore] public bool IsAutoMode { set; get; }

    /// <summary>
    /// 取得或设置实体模型信息。
    /// </summary>
    [JsonIgnore] public EntityInfo Entity { get; set; }

    /// <summary>
    /// 取得或设置页面模型信息。
    /// </summary>
    [JsonIgnore] public PageInfo Page { get; set; }

    /// <summary>
    /// 取得或设置表单模型信息。
    /// </summary>
    [JsonIgnore] public FormInfo Form { get; set; }

    /// <summary>
    /// 取得模型类名称。
    /// </summary>
    [JsonIgnore] public string ModelName => $"{Code}Info";

    /// <summary>
    /// 取得模型类路径。
    /// </summary>
    [JsonIgnore] public string ModelPath => $"Models/{ModelName}.cs";

    /// <summary>
    /// 取得实体类名称。
    /// </summary>
    [JsonIgnore] public string EntityName => $"{Prefix}{Code}";

    /// <summary>
    /// 取得实体类路径。
    /// </summary>
    [JsonIgnore] public string EntityPath => $"Entities/{EntityName}.cs";

    /// <summary>
    /// 取得页面类名称。
    /// </summary>
    [JsonIgnore] public string PageName => $"{Code}List";

    /// <summary>
    /// 取得页面类路径。
    /// </summary>
    [JsonIgnore] public string PagePath => $"Pages/{PageName}.cs";

    /// <summary>
    /// 取得表单类名称。
    /// </summary>
    [JsonIgnore] public string FormName => $"{Code}Form";

    /// <summary>
    /// 取得表单类路径。
    /// </summary>
    [JsonIgnore] public string FormPath => $"Pages/{FormName}.razor";

    /// <summary>
    /// 取得服务类名称。
    /// </summary>
    [JsonIgnore] public string ServiceName => $"{Code}Service";

    /// <summary>
    /// 取得服务接口路径。
    /// </summary>
    [JsonIgnore] public string ServiceIPath => $"Services/{ServiceName}.cs";

    /// <summary>
    /// 取得服务实现类路径。
    /// </summary>
    [JsonIgnore] public string ServicePath => $"Services/{ServiceName}.cs";

    /// <summary>
    /// 取得是否包含附件字段。
    /// </summary>
    [JsonIgnore] public bool HasFile => Fields.Any(f => f.Type == FieldType.File);

    internal void TransModels()
    {
        Entity = ToEntity();
        Page = ToPage();
        Form = ToForm();
    }

    private EntityInfo ToEntity()
    {
        var info = new EntityInfo
        {
            Id = Code,
            Name = Name,
            PageUrl = PageUrl,
            Fields = [.. Fields.Select(f => f.ToField())]
        };
        return info;
    }

    private PageInfo ToPage()
    {
        var info = new PageInfo();
        info.Tools = Functions?.Where(f => f != "Edit" && f != "Delete").Select(d => new ActionInfo(d)).ToList();
        info.Actions = Functions?.Where(f => f == "Edit" || f == "Delete").Select(d => new ActionInfo(d)).ToList();
        return info;
    }

    private FormInfo ToForm()
    {
        var info = new FormInfo();
        info.Fields = [.. Fields.Where(f => f.IsForm).Select(f => f.ToFormField())];
        return info;
    }
}