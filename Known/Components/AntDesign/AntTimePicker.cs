using AntDesign;

namespace Known.Components;

/// <summary>
/// 扩展Ant时间选择框组件类。
/// </summary>
public class AntTimePicker : TimePicker<TimeOnly?>
{
    [CascadingParameter] private IComContainer AntForm { get; set; }
    [CascadingParameter] private DataItem Item { get; set; }

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        if (AntForm != null)
            Disabled = AntForm.IsView;
        if (Item != null)
            Item.Type = typeof(DateTime?);
        AutoFocus = false;
        base.OnInitialized();
    }
}