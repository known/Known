namespace Known.AntBlazor.Components;

/// <summary>
/// 扩展Ant多选框列表组件类。
/// </summary>
public class AntCheckboxGroup : CheckboxGroup<string>
{
    [CascadingParameter] private IAntForm AntForm { get; set; }
    [CascadingParameter] private DataItem Item { get; set; }

    /// <summary>
    /// 取得或设置列表组件关联的数据字典类别名或可数项目（用逗号分割，如：项目1,项目2）。
    /// </summary>
    [Parameter] public string Category { get; set; }

    /// <summary>
    /// 取得或设置列表组件关联的代码表列表集合。
    /// </summary>
    [Parameter] public List<CodeInfo> Codes { get; set; }

    /// <summary>
    /// 初始化组件。
    /// </summary>
    protected override void OnInitialized()
    {
        if (AntForm != null)
            Disabled = AntForm.IsView;
        if (Item != null)
            Item.Type = typeof(string);
        if (!string.IsNullOrWhiteSpace(Category))
            Codes = Cache.GetCodes(Category);
        base.OnInitialized();
    }

    /// <summary>
    /// 异步设置组件参数。
    /// </summary>
    /// <returns></returns>
    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
        Options = Codes.ToCheckboxOptions(o =>
        {
            o.Checked = Value != null && Value.Contains(o.Value);
        });
    }
}