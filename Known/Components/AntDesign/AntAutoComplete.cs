using AntDesign;

namespace Known.Components;

/// <summary>
/// 扩展Ant自动完成下拉框组件。
/// </summary>
public class AntAutoComplete : AutoComplete<CodeInfo>
{
    [CascadingParameter] private DataItem Item { get; set; }

    /// <summary>
    /// 取得或设置表单容器对象。
    /// </summary>
    [CascadingParameter] protected IComContainer AntForm { get; set; }

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        if (AntForm != null)
            Disabled = AntForm.IsView;
        if (Item != null)
        {
            Item.Type = typeof(string);
            Placeholder = Item.Language[Language.PleaseSelectInput];
        }
        AllowFilter = true;
        OptionFormat = item => item.Value.Name;
        base.OnInitialized();
    }
}