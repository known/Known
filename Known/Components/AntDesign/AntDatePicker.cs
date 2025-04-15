using AntDesign;
using OneOf;

namespace Known.Components;

/// <summary>
/// 扩展Ant日期选择框组件类。
/// </summary>
public class AntDatePicker : DatePicker<DateTime?>
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

/// <summary>
/// 扩展Ant日期时间选择框组件类。
/// </summary>
public class AntDateTimePicker : DatePicker<DateTime?>
{
    /// <summary>
    /// 构造函数，创建一个扩展Ant日期时间选择框组件类的实例。
    /// </summary>
    public AntDateTimePicker()
    {
        var format = "yyyy-MM-dd HH:mm";
        ShowTime = true;
        Format = format;
        Mask = format;
        Placeholder = OneOf<string, string[]>.FromT0(format);
    }

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