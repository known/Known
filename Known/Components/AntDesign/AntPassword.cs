using AntDesign;

namespace Known.Components;

/// <summary>
/// 扩展Ant密码输入框组件类。
/// </summary>
public class AntPassword : InputPassword
{
    [CascadingParameter] private IComContainer AntForm { get; set; }
    [CascadingParameter] private DataItem Item { get; set; }

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
        if (!string.IsNullOrWhiteSpace(Icon))
            Prefix = b => b.Icon(Icon);
        base.OnInitialized();
    }
}