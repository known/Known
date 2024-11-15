using AntDesign;

namespace Known.Components;

/// <summary>
/// 扩展Ant自动完成下拉框组件。
/// </summary>
public class AntAutoComplete : AutoComplete<CodeInfo>
{
    [CascadingParameter] private IAntForm AntForm { get; set; }
    [CascadingParameter] private DataItem Item { get; set; }

    /// <summary>
    /// 初始化组件。
    /// </summary>
    protected override void OnInitialized()
    {
        if (AntForm != null)
            Disabled = AntForm.IsView;
        if (Item != null)
        {
            Item.Type = typeof(string);
            Placeholder = Item.Language.GetString("PleaseSelectInput");
        }
        AllowFilter = true;
        OptionFormat = item => item.Value.Name;
        base.OnInitialized();
    }
}