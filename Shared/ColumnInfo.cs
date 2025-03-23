namespace Known;

/// <summary>
/// 栏位信息类。
/// </summary>
public partial class ColumnInfo
{
    /// <summary>
    /// 构造函数，创建一个栏位信息类的实例。
    /// </summary>
    public ColumnInfo() { }

    internal ColumnInfo(ColumnAttribute attr)
    {
        SetColumnAttribute(attr);
        SetPropertyInfo(attr.Property);
    }

    /// <summary>
    /// 构造函数，创建一个栏位信息类的实例。
    /// </summary>
    /// <param name="info">栏位属性对象。</param>
    public ColumnInfo(PropertyInfo info)
    {
        var column = info?.GetCustomAttribute<ColumnAttribute>();
        SetColumnAttribute(column);
        SetPropertyInfo(info);
    }

    /// <summary>
    /// 取得或设置栏位ID。
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 取得或设置栏位名称。
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置栏位提示文字。
    /// </summary>
    public string Tooltip { get; set; }

    /// <summary>
    /// 取得或设置栏位是否可见，默认True。
    /// </summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>
    /// 取得或设置栏位备注。
    /// </summary>
    public string Note { get; set; }

    /// <summary>
    /// 取得栏位关联的对象属性。
    /// </summary>
    public PropertyInfo Property { get; internal set; }

    private void SetPropertyInfo(PropertyInfo info)
    {
        if (info == null)
            return;

        Property = info;
        Id = info.Name;
        Required = info.IsRequired();

        var form = info.GetCustomAttribute<FormAttribute>();
        if (form != null)
            SetFormAttribute(form);

        SetColumnInfo(info);
    }

    internal void SetColumnInfo(PropertyInfo info)
    {
        if (info == null)
            return;

        if (Type == FieldType.Text)
            Type = info.GetFieldType();

        DisplayName = info.DisplayName();
        if (string.IsNullOrWhiteSpace(Name))
            Name = DisplayName ?? info.Name;

        var category = info.Category();
        if (!string.IsNullOrWhiteSpace(category))
            Category = category;
    }

    private void SetColumnAttribute(ColumnAttribute attr)
    {
        if (attr == null)
            return;

        IsViewLink = attr.IsViewLink;
        IsQuery = attr.IsQuery;
        IsQueryAll = attr.IsQueryAll;
        if (attr.Type != FieldType.Text)
            Type = attr.Type;
        Category = attr.Category;
        IsSum = attr.IsSum;
        IsSort = attr.IsSort;
        DefaultSort = attr.DefaultSort;
        Fixed = attr.Fixed;
        Width = attr.Width;
        Align = attr.Align;
    }

    private void SetFormAttribute(FormAttribute attr)
    {
        if (attr == null)
            return;

        IsForm = true;
        Row = attr.Row;
        Column = attr.Column;
        if (!string.IsNullOrWhiteSpace(attr.Type))
            Type = Utils.ConvertTo<FieldType>(attr.Type);
        if (Type == FieldType.Custom)
            CustomField = attr.CustomField;
        ReadOnly = attr.ReadOnly;
        Placeholder = attr.Placeholder;
        if (string.IsNullOrWhiteSpace(Placeholder) && Type == FieldType.Select)
            Placeholder = "请选择";
    }
}