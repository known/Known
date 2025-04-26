namespace Known.Internals;

/// <summary>
/// 更多表单属性设置组件类。
/// </summary>
public partial class MoreFormSetting
{
    private readonly List<CodeInfo> CustomTypes = [.. Config.FieldTypes.Select(c => new CodeInfo(c.Key, c.Key))];

    /// <summary>
    /// 取得或设置是否支持默认值设置。
    /// </summary>
    [Parameter] public bool IsDefaultValue { get; set; }
}