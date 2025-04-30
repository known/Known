using AntDesign;

namespace Known.Components;

/// <summary>
/// 按钮扩展组件类。
/// </summary>
public partial class KButton
{
    private bool isLoad;

    /// <summary>
    /// 取得或设置是否是块级按钮。
    /// </summary>
    [Parameter] public bool Block { get; set; }

    /// <summary>
    /// 取得或设置是否是危险状态。
    /// </summary>
    [Parameter] public bool Danger { get; set; }

    /// <summary>
    /// 取得或设置按钮图标。
    /// </summary>
    [Parameter] public string Icon { get; set; }

    /// <summary>
    /// 取得或设置按钮类型（如：ButtonType.Primary），默认Primary。
    /// </summary>
    [Parameter] public ButtonType Type { get; set; } = ButtonType.Primary;

    /// <summary>
    /// 取得或设置按钮提示信息。
    /// </summary>
    [Parameter] public string Title { get; set; }

    /// <summary>
    /// 取得或设置徽章数量。
    /// </summary>
    [Parameter] public int Badge { get; set; }

    /// <summary>
    /// 取得或设置按钮单击事件。
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> OnClick { get; set; }

    private async Task OnButtonClickAsync(MouseEventArgs args)
    {
        if (isLoad || !OnClick.HasDelegate)
            return;

        isLoad = true;
        await OnClick.InvokeAsync(args);
        isLoad = false;
    }
}