namespace Known.Pages;

/// <summary>
/// 系统设置表单组件类。
/// </summary>
public class SettingForm : BaseForm<SettingInfo>
{
    private ISettingService Service;

    /// <summary>
    /// 取得或设置系统设置改变方法委托。
    /// </summary>
    [Parameter] public Action<SettingInfo> OnChanged { get; set; }

    /// <summary>
    /// 异步初始化设置表单。
    /// </summary>
    /// <returns></returns>
    protected override async Task OnInitFormAsync()
    {
        await base.OnInitFormAsync();
        Service = await CreateServiceAsync<ISettingService>();
        Model = new FormModel<SettingInfo>(this) { Data = Context.UserSetting };
        Model.AddRow().AddColumn(c => c.MultiTab);
        Model.AddRow().AddColumn(c => c.Accordion);
        Model.AddRow().AddColumn(c => c.Collapsed);
        Model.AddRow().AddColumn(c => c.MenuTheme, c =>
        {
            c.Category = "Light,Dark";
            c.Type = FieldType.RadioList;
        });
    }

    /// <summary>
    /// 呈现设置表单内容。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    protected override void BuildRender(RenderTreeBuilder builder)
    {
        builder.Div("kui-form-setting", () =>
        {
            base.BuildForm(builder);
            builder.Div("kui-center", () =>
            {
                builder.Button(Language.Save, this.Callback<MouseEventArgs>(SaveAsync));
                builder.Button(Language.Reset, this.Callback<MouseEventArgs>(ResetAsync), "default");
            });
        });
    }

    private async void SaveAsync(MouseEventArgs arg)
    {
        var result = await Service.SaveUserSettingInfoAsync(Model.Data);
        if (result.IsValid)
        {
            Context.UserSetting = Model.Data;
            OnChanged?.Invoke(Context.UserSetting);
            await App?.StateChangedAsync();
        }
    }

    private async void ResetAsync(MouseEventArgs arg)
    {
        Model.Data.Reset();
        var result = await Service.SaveUserSettingInfoAsync(Model.Data);
        if (result.IsValid)
        {
            Context.UserSetting = Model.Data;
            OnChanged?.Invoke(Context.UserSetting);
            await App?.StateChangedAsync();
        }
    }
}