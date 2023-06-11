namespace Known.Razor.Pages;

public class SettingForm : BaseForm<SettingInfo>
{
    private bool isEdit = false;
    private readonly string sizes = string.Join(",", PagingCriteria.PageSizes);

    public SettingForm()
    {
        Style = "";
    }

    [Parameter] public string Title { get; set; }

    protected override void OnInitialized()
    {
        Model = Setting.Info;
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (!string.IsNullOrEmpty(Title))
            builder.Div("title", Title);
        base.BuildRenderTree(builder);
    }

    protected override void BuildFields(FieldBuilder<SettingInfo> builder)
    {
        builder.Field<Input>("主题色", nameof(SettingInfo.ThemeColor))
               .ReadOnly(!isEdit)
               .Set(f => f.Type, "color")
               .Set(f => f.ValueChanged, value => PageAction.RefreshThemeColor?.Invoke(value))
               .Build();
        builder.Field<CheckBox>("随机色", nameof(SettingInfo.RandomColor)).ReadOnly(!isEdit).Set(f => f.Switch, true).Build();
        builder.Field<CheckBox>("标签页", nameof(SettingInfo.MultiTab)).ReadOnly(!isEdit).Set(f => f.Switch, true).Build();
        builder.Field<Select>("每页大小", nameof(SettingInfo.PageSize)).ReadOnly(!isEdit).Set(f => f.Codes, sizes).Build();
    }

    protected override void BuildButtons(RenderTreeBuilder builder)
    {
        if (!isEdit)
        {
            builder.Button(FormButton.Edit, Callback(e => isEdit = true));
        }
        else
        {
            builder.Button(FormButton.Save, Callback(OnSave));
            builder.Button(FormButton.Cancel, Callback(e => isEdit = false));
        }
    }

    private void OnSave()
    {
        SubmitAsync(data =>
        {
            data.RandomColor = data.RandomColor == "True";
            data.MultiTab = data.MultiTab == "True";
            var info = new SettingFormInfo
            {
                Type = UserSetting.KeyInfo,
                Name = "用户设置",
                Data = Utils.ToJson(data)
            };
            Setting.UserSetting.Info = Utils.FromJson<SettingInfo>(info.Data);
            return Platform.User.SaveSettingAsync(info);
        }, result =>
        {
            isEdit = false;
            StateChanged();
        });
    }
}