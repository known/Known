using Known.Extensions;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace Known.Blazor;

public class SettingForm : BaseForm<SettingInfo>
{
    protected override async Task OnInitFormAsync()
    {
        Model = new FormModel<SettingInfo>(Context)
        {
            LabelSpan = 12,
            Data = Context.UserSetting
        };
        await base.OnInitFormAsync();
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
        var result = await Platform.Setting.SaveUserSettingAsync(SettingInfo.KeyInfo, Model.Data);
        if (result.IsValid)
        {
            Context.UserSetting = Model.Data;
            Context.RefreshPage();
        }
    }

    private async void ResetAsync(MouseEventArgs arg)
    {
        var result = await Platform.Setting.DeleteUserSettingAsync(SettingInfo.KeyInfo);
        if (result.IsValid)
        {
            Model.Data = new();
            Context.UserSetting = Model.Data;
            Context.RefreshPage();
        }
    }
}