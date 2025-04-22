namespace Known.Components;

/// <summary>
/// 移动端日期选择器。
/// </summary>
public class AppDatePicker : InputDate<DateTime?>
{
    /// <summary>
    /// 初始化一个新的 <see cref="AppDatePicker"/> 类实例。
    /// </summary>
    public AppDatePicker()
    {
        Type = InputDateType.Date;
    }
}

/// <summary>
/// 移动端日期时间选择器。
/// </summary>
public class AppDateTimePicker : InputDate<DateTime?>
{
    /// <summary>
    /// 初始化一个新的 <see cref="AppDateTimePicker"/> 类实例。
    /// </summary>
    public AppDateTimePicker()
    {
        Type = InputDateType.DateTimeLocal;
    }
}