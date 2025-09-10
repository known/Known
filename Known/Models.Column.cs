namespace Known;

public partial class ColumnInfo
{
    internal ColumnInfo(string id, RenderFragment template)
    {
        Id = id;
        Template = template;
    }

    /// <summary>
    /// 取得或设置表单字段呈现模板。
    /// </summary>
    public RenderFragment Template { get; set; }

    /// <summary>
    /// 取得或设置栏位是否是过滤字段，默认过滤。
    /// </summary>
    public bool IsFilter { get; set; } = true;

    /// <summary>
    /// 取得或设置栏位是否支持过滤字段查询类型，默认支持。
    /// </summary>
    public bool IsFilterType { get; set; } = true;

    /// <summary>
    /// 取得或设置表单字段过滤条件模板。
    /// </summary>
    public RenderFragment FilterTemplate { get; set; }

    internal ColumnInfo(PageColumnInfo info) => SetPageColumnInfo(info);
    private void SetPageColumnInfo(PageColumnInfo info)
    {
        if (info == null)
            return;

        Id = info.Id;
        Name = info.Name ?? info.Id;
        IsViewLink = info.IsViewLink;
        IsQuery = info.IsQuery;
        IsQueryAll = info.IsQueryAll;
        QueryValue = info.QueryValue;
        Type = info.Type;
        Category = info.Category;
        Ellipsis = info.Ellipsis;
        IsSum = info.IsSum;
        IsSort = info.IsSort;
        DefaultSort = info.DefaultSort;
        Fixed = info.Fixed;
        Width = info.Width;
        Align = info.Align;
        Position = info.Position;

        if (info.Id == nameof(EntityBase.CreateTime) || info.Id == nameof(EntityBase.ModifyTime))
            Type = FieldType.Date;
    }

    internal ColumnInfo(FormFieldInfo info) => SetFormFieldInfo(info);
    internal void SetFormFieldInfo(FormFieldInfo info)
    {
        if (info == null)
            return;

        Id = info.Id;
        Name = info.Name ?? info.Id;
        Row = info.Row;
        Column = info.Column;
        Span = info.Span;
        Type = info.Type;
        CustomField = info.CustomField;
        MultiFile = info.MultiFile;
        ReadOnly = info.ReadOnly;
        Required = info.Required;
        Placeholder = info.Placeholder;
        FieldValue = info.FieldValue;
        Rows = info.Rows;
        Unit = info.Unit;
        Category = info.Category;
    }
}