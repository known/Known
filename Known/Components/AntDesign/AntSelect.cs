using AntDesign;

namespace Known.Components;

/// <summary>
/// 扩展Ant选择框组件类。
/// </summary>
public class AntSelect : Select<string, string>
{
    [CascadingParameter] private DataItem Item { get; set; }

    /// <summary>
    /// 取得或设置表单容器对象。
    /// </summary>
    [CascadingParameter] protected IComContainer AntForm { get; set; }

    /// <summary>
    /// 取得或设置UI上下文对象级联值实例。
    /// </summary>
    [CascadingParameter] public UIContext Context { get; set; }

    /// <summary>
    /// 取得或设置前缀图标。
    /// </summary>
    [Parameter] public string Icon { get; set; }

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        if (AntForm != null)
            Disabled = AntForm.IsView;
        if (Item != null)
            Item.Type = typeof(string);
        AutoFocus = false;
        EnableVirtualization = true;
        EnableSearch = true;
        base.OnInitialized();
    }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        var emptyText = "";
        if (Item != null)
            emptyText = Item.Language[Language.PleaseSelect];
        if (string.IsNullOrEmpty(Placeholder))
            Placeholder = emptyText;
        if (!string.IsNullOrWhiteSpace(Icon))
            PrefixIcon = b => b.Span().Style("padding:0 9px 0 7px;").Child(() => b.Icon(Icon));
        base.OnParametersSet();
    }
}

/// <summary>
/// 扩展Ant代码表选择框组件类。
/// </summary>
public class AntSelectCode : Select<string, CodeInfo>
{
    [CascadingParameter] private DataItem Item { get; set; }

    /// <summary>
    /// 取得或设置表单容器对象。
    /// </summary>
    [CascadingParameter] protected IComContainer AntForm { get; set; }

    /// <summary>
    /// 取得或设置UI上下文对象级联值实例。
    /// </summary>
    [CascadingParameter] public UIContext Context { get; set; }

    /// <summary>
    /// 取得或设置选择框组件关联的数据字典类别名或可数项目（用逗号分割，如：项目1,项目2）。
    /// </summary>
    [Parameter] public string Category { get; set; }

    /// <summary>
    /// 取得或设置选择框组件显示文本格式，比如：{Code}-{Name}，默认只显示名称。
    /// </summary>
    [Parameter] public string LabelFormat { get; set; }

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        if (AntForm != null)
            Disabled = AntForm.IsView;
        if (Item != null)
            Item.Type = typeof(string);
        AutoFocus = false;
        ValueName = nameof(CodeInfo.Code);
        LabelName = nameof(CodeInfo.Name);
        EnableVirtualization = true;
        EnableSearch = true;
        AllowClear = true;
        base.OnInitialized();
    }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        var emptyText = "";
        if (Item != null)
            emptyText = Item.Language[Language.PleaseSelect];
        if (string.IsNullOrEmpty(Placeholder))
            Placeholder = emptyText;
        if (!string.IsNullOrWhiteSpace(Category))
            DataSource = Cache.GetCodes(Category, LabelFormat, Item?.Language).ToCodes(emptyText);
        base.OnParametersSet();
    }
}

/// <summary>
/// 扩展Ant枚举选择框组件类。
/// </summary>
/// <typeparam name="T"></typeparam>
public class AntSelectEnum<T> : EnumSelect<T>
{
    [CascadingParameter] private DataItem Item { get; set; }

    /// <summary>
    /// 取得或设置表单容器对象。
    /// </summary>
    [CascadingParameter] protected IComContainer AntForm { get; set; }

    /// <summary>
    /// 取得或设置UI上下文对象级联值实例。
    /// </summary>
    [CascadingParameter] public UIContext Context { get; set; }

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        if (AntForm != null)
            Disabled = AntForm.IsView;
        base.OnInitialized();
    }

    /// <inheritdoc />
    protected override string GetLabel(T item)
    {
        var label = base.GetLabel(item);
        return Context?.Language[label];
    }
}