namespace Known.Internals;

/// <summary>
/// 多语言设置组件类。
/// </summary>
public partial class LanguageSetting
{
    /// <summary>
    /// 取得或设置语言设置信息列表。
    /// </summary>
    [Parameter] public List<LanguageSettingInfo> DataSource { get; set; }

    internal void Reset()
    {
        DataSource = Language.GetDefaultSettings();
        StateChanged();
    }

    private void OnDefaultChange(LanguageSettingInfo item, bool isDefault)
    {
        foreach (var setting in DataSource)
        {
            if (isDefault && setting.Id == item.Id)
                setting.Default = isDefault;
            else
                setting.Default = false;
        }
    }
}