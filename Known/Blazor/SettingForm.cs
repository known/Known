namespace Known.Blazor;

public class SettingForm : BaseForm<SettingInfo>
{
    private ISettingService settingService;

    protected override async Task OnInitFormAsync()
    {
        await base.OnInitFormAsync();
        settingService = await Factory.CreateAsync<ISettingService>(Context);
        Model = new FormModel<SettingInfo>(Context, true)
        {
            LabelSpan = 12,
            Data = Context.UserSetting
        };
    }

    protected override void BuildRender(RenderTreeBuilder builder)
    {
        builder.Div("kui-form-setting", () =>
        {
            base.BuildForm(builder);
            builder.Div("center", () =>
            {
                UI.Button(builder, Language.Save, this.Callback<MouseEventArgs>(SaveAsync), "primary");
                UI.Button(builder, Language.Reset, this.Callback<MouseEventArgs>(ResetAsync));
            });
        });
    }

    private async void SaveAsync(MouseEventArgs arg)
    {
        var result = await settingService.SaveUserSettingAsync(SettingInfo.KeyInfo, Model.Data);
        if (result.IsValid)
        {
            Context.UserSetting = Model.Data;
            App?.StateChanged();
        }
    }

    private async void ResetAsync(MouseEventArgs arg)
    {
        var result = await settingService.DeleteUserSettingAsync(SettingInfo.KeyInfo);
        if (result.IsValid)
        {
            Model.Data = new();
            Context.UserSetting = Model.Data;
            App?.StateChanged();
        }
    }
}