namespace Known.Internals;

/// <summary>
/// 系统设置抽屉表单组件类。
/// </summary>
public partial class SettingForm
{
    private readonly Dictionary<string, string> Colors = new()
    {
        ["default"] = "#1890ff",
        ["dust"] = "#F5222D",
        ["volcano"] = "#FA541C",
        ["sunset"] = "#FAAD14",
        ["cyan"] = "#13C2C2",
        ["green"] = "#52C41A",
        ["geekblue"] = "#2F54EB",
        ["purple"] = "#722ED1"
    };
    private static List<CodeInfo> MenuThemes => [
        new CodeInfo("Dark", Language.Dark),
        new CodeInfo("Light", Language.Light)
    ];
    private static List<CodeInfo> LayoutModes => [
        new CodeInfo("Vertical", Language.Vertical),
        new CodeInfo("Horizontal", Language.Horizontal),
        new CodeInfo("Float", Language.Float)
    ];

    /// <summary>
    /// 取得或设置系统设置项目改变事件委托。
    /// </summary>
    [Parameter] public Action OnChange { get; set; }

    /// <summary>
    /// 取得或设置保存系统设置委托。
    /// </summary>
    [Parameter] public Func<Task> OnSave { get; set; }

    /// <summary>
    /// 取得或设置重置系统设置委托。
    /// </summary>
    [Parameter] public Func<Task> OnReset { get; set; }

    private async Task OnThemeColor(string theme)
    {
        Context.UserSetting.ThemeColor = theme;
        var href = $"_content/Known/css/theme/{theme}.css";
        await JS.SetStyleSheetAsync("/theme/", href);
    }

    private async Task OnResetAsync()
    {
        if (OnReset != null)
        {
            await OnReset.Invoke();
            await StateChangedAsync();
            OnChange?.Invoke();
        }
    }
}