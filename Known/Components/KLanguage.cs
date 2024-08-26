namespace Known.Components;

public class KLanguage : BaseComponent
{
    private ActionInfo current;
    private ISettingService Service;

    [Parameter] public string Icon { get; set; } = "translation";

    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        Service = await CreateServiceAsync<ISettingService>();
        current = Language.GetLanguage(Context.CurrentLanguage);
    }

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
            await Service.SaveUserSettingAsync(Context.UserSetting);
        }
        await JS.SetCurrentLanguageAsync(current.Id);
        Navigation.Refresh(true);
    }
}