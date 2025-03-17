using AntDesign;

namespace Known.Components;

/// <summary>
/// 扩展Ant单选按钮列表组件类。
/// </summary>
public class AntRadioGroup : RadioGroup<string>
{
    [CascadingParameter] private IAntForm AntForm { get; set; }
    [CascadingParameter] private DataItem Item { get; set; }

    /// <summary>
    /// 取得或设置列表组件关联的数据字典类别名或可数项目（用逗号分割，如：男,女）。
    /// </summary>
    [Parameter] public string Category { get; set; }

    /// <summary>
    /// 取得或设置列表组件关联的代码表列表集合。
    /// </summary>
    [Parameter] public List<CodeInfo> Codes { get; set; }

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        if (AntForm != null)
            Disabled = AntForm.IsView;
        if (Item != null)
            Item.Type = typeof(string);
        base.OnInitialized();
    }

    /// <inheritdoc />
    protected override async Task OnParametersSetAsync()
    {
        if (!string.IsNullOrWhiteSpace(Category))
            Codes = Cache.GetCodes(Category);
        Options = Codes.ToRadioOptions();
        await base.OnParametersSetAsync();
        //Fixed单选按钮组切换不刷新问题
        //OnChange = EventCallback.Factory.Create<string>(this, value => StateHasChanged());
    }
}