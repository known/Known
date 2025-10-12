using AntDesign;

namespace Known.Components;

/// <summary>
/// 提示框扩展组件类。
/// </summary>
public class KAlert : BaseComponent
{
    /// <summary>
    /// 取得或设置提示框类型。
    /// </summary>
    [Parameter] public StyleType Type { get; set; }

    /// <summary>
    /// 取得或设置提示框文本。
    /// </summary>
    [Parameter] public string Text { get; set; }

    /// <inheritdoc />
    protected override void BuildRender(RenderTreeBuilder builder)
    {
        if (!Visible)
            return;

        builder.Component<Alert>()
               .Set(c => c.ShowIcon, true)
               .Set(c => c.Style, "margin-bottom:10px;")
               .Set(c => c.Type, GetAlertType())
               .Set(c => c.Message, Language[Text])
               .Build();
    }

    private AlertType GetAlertType()
    {
        return Type switch
        {
            StyleType.Success => AlertType.Success,
            StyleType.Info => AlertType.Info,
            StyleType.Warning => AlertType.Warning,
            StyleType.Error => AlertType.Error,
            _ => AlertType.Default
        };
    }
}