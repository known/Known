namespace Known.Components;

/// <summary>
/// 多语言下拉框组件类。
/// </summary>
public class KLanguage : BaseComponent
{
    private ActionInfo current;
    private ISettingService Service;

    /// <summary>
    /// 取得或设置下拉框图标，默认为translation。
    /// </summary>
    [Parameter] public string Icon { get; set; } = "translation";

    /// <summary>
    /// 异步初始化多语言下拉框组件。
    /// </summary>
    /// <returns></returns>
    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        Service = await CreateServiceAsync<ISettingService>();
        current = Language.GetLanguage(Context.CurrentLanguage);
    }

    /// <summary>
    /// 呈现多语言下拉框组件内容。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    protected override void BuildRender(RenderTreeBuilder builder)
    {
        UI.BuildDropdown(builder, new DropdownModel
        {
            Icon = Icon,
            Text = current?.Icon,
            Items = Language.Items.Where(i => i.Visible).ToList(),
            OnItemClick = OnLanguageChanged
        });
    }

    private async void OnLanguageChanged(ActionInfo info)
    {
        current = info;
        Context.CurrentLanguage = current.Id;
        if (CurrentUser != null)
        {
            Context.UserSetting.Language = current.Id;
            await Service.SaveUserSettingInfoAsync(Context.UserSetting);
        }
        await JS.SetCurrentLanguageAsync(current.Id);
        Navigation.Refresh(true);
    }
}