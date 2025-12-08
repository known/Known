using AntDesign;
using OneOf;

namespace Known.Components;

/// <summary>
/// 扩展Ant日期选择框组件类。
/// </summary>
public class AntDatePicker : DatePicker<DateTime?>
{
    [CascadingParameter] private DataItem Item { get; set; }

    /// <summary>
    /// 取得或设置表单容器对象。
    /// </summary>
    [CascadingParameter] protected IComContainer AntForm { get; set; }

    /// <summary>
    /// 取得或设置UI上下文对象级联值实例。
    /// </summary>
    [CascadingParameter] public UIContext Context { get; set; }

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        if (AntForm != null)
            Disabled = AntForm.IsView;
        if (Item != null)
            Item.Type = typeof(DateTime?);
        Picker = DatePickerType.Date;
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
        var format = Config.DateTimeFormat;
        ShowTime = true;
        Format = format;
        Mask = format;
        Placeholder = OneOf<string, string[]>.FromT0(format);
    }

    [CascadingParameter] private DataItem Item { get; set; }

    /// <summary>
    /// 取得或设置表单容器对象。
    /// </summary>
    [CascadingParameter] protected IComContainer AntForm { get; set; }

    /// <summary>
    /// 取得或设置UI上下文对象级联值实例。
    /// </summary>
    [CascadingParameter] public UIContext Context { get; set; }

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        if (AntForm != null)
            Disabled = AntForm.IsView;
        if (Item != null)
            Item.Type = typeof(DateTime?);
        base.OnInitialized();
    }
}