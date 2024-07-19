namespace Known.Pages;

public class SettingForm : BaseForm<SettingInfo>
{
    private ISettingService Service;

    protected override async Task OnInitFormAsync()
    {
        await base.OnInitFormAsync();
        Service = await CreateServiceAsync<ISettingService>();
        Model = new FormModel<SettingInfo>(Context, true)
        {
            Info = new FormInfo { LabelSpan = 12 },
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
                builder.Button(Language.Save, this.Callback<MouseEventArgs>(SaveAsync), "primary");
                builder.Button(Language.Reset, this.Callback<MouseEventArgs>(ResetAsync));
            });
        });
    }

    private async void SaveAsync(MouseEventArgs arg)
    {
        var result = await Service.SaveUserSettingAsync(SettingInfo.KeyInfo, Model.Data);
        if (result.IsValid)
        {
            Context.UserSetting = Model.Data;
            await App?.StateChangedAsync();
        }
    }

    private async void ResetAsync(MouseEventArgs arg)
    {
        var result = await Service.DeleteUserSettingAsync(SettingInfo.KeyInfo);
        if (result.IsValid)
        {
            Model.Data = new();
            Context.UserSetting = Model.Data;
            await App?.StateChangedAsync();
        }
    }
}