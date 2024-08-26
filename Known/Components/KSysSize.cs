namespace Known.Components;

public class KSysSize : BaseComponent
{
    private ISettingService Service;

    [Parameter] public string Icon { get; set; } = "font-size";
    [Parameter] public List<ActionInfo> Items { get; set; }

    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        Service = await CreateServiceAsync<ISettingService>();
        UIConfig.Sizes.ForEach(s => s.Name = Language[$"Nav.Size{s.Id}"]);
    }

    protected override void BuildRender(RenderTreeBuilder builder)
    {
        UI.BuildDropdown(builder, new DropdownModel
        {
            Icon = Icon,
            Items = UIConfig.Sizes,
            OnItemClick = OnSizeChanged
        });
    }

    private async void OnSizeChanged(ActionInfo info)
    {
        Context.UserSetting.Size = info.Id;
        await Service.SaveUserSettingAsync(Context.UserSetting);
        await JS.SetCurrentSizeAsync(info.Id);
        Navigation.Refresh(true);
    }
}