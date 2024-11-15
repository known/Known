using AntDesign;
using OneOf;

namespace Known.Components;

/// <summary>
/// 扩展Ant日期选择框组件类。
/// </summary>
public class AntDatePicker : DatePicker<DateTime?>
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
            Item.Type = typeof(DateTime?);
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
        AutoFocus = true;
        ShowTime = true;
        Format = format;
        Mask = format;
        Placeholder = OneOf<string, string[]>.FromT0(format);
    }

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
            Item.Type = typeof(DateTime?);
        base.OnInitialized();
    }
}