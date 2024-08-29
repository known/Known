﻿namespace Known.Pages;

public class SettingForm : BaseForm<SettingInfo>
{
    private ISettingService Service;

    protected override async Task OnInitFormAsync()
    {
        await base.OnInitFormAsync();
        Service = await CreateServiceAsync<ISettingService>();
        Model = new FormModel<SettingInfo>(this, true)
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
                builder.Button(Language.Save, this.Callback<MouseEventArgs>(SaveAsync));
                builder.Button(Language.Reset, this.Callback<MouseEventArgs>(ResetAsync), "default");
            });
        });
    }

    private async void SaveAsync(MouseEventArgs arg)
    {
        var result = await Service.SaveUserSettingAsync(Model.Data);
        if (result.IsValid)
        {
            Context.UserSetting = Model.Data;
            await App?.StateChangedAsync();
        }
    }

    private async void ResetAsync(MouseEventArgs arg)
    {
        Model.Data.Reset();
        var result = await Service.SaveUserSettingAsync(Model.Data);
        if (result.IsValid)
        {
            Context.UserSetting = Model.Data;
            await App?.StateChangedAsync();
        }
    }
}