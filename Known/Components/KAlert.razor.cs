using AntDesign;

namespace Known.Components;

/// <summary>
/// 提示框扩展组件类。
/// </summary>
public partial class KAlert
{
    /// <summary>
    /// 取得或设置提示框类型。
    /// </summary>
    [Parameter] public StyleType Type { get; set; }

    /// <summary>
    /// 取得或设置提示框文本。
    /// </summary>
    [Parameter] public string Text { get; set; }

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