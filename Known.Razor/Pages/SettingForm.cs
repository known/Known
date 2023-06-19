namespace Known.Razor.Pages;

public class SettingForm : BaseForm<SettingInfo>
{
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
               .Set(f => f.Type, "color")
               .Set(f => f.ValueChanged, OnHeadColorChanged)
               .Build();
        builder.Field<Input>("侧栏色", nameof(SettingInfo.SideColor))
               .Set(f => f.Type, "color")
               .Set(f => f.ValueChanged, OnSideColorChanged)
               .Build();
        builder.Field<CheckBox>("随机色", nameof(SettingInfo.RandomColor)).Set(f => f.Switch, true).Build();
        builder.Field<CheckBox>("标签页", nameof(SettingInfo.MultiTab)).Set(f => f.Switch, true).Build();
        builder.Field<Select>("每页大小", nameof(SettingInfo.PageSize)).Set(f => f.Codes, sizes).Build();
    }

    protected override void BuildButtons(RenderTreeBuilder builder)
    {
        builder.Button(FormButton.Save, Callback(OnSave));
        builder.Button(FormButton.Reset, Callback(OnReset));
    }

    private void OnHeadColorChanged(string color)
    {
        Setting.Info.ThemeColor = color;
        PageAction.RefreshHeadColor?.Invoke();
        PageAction.RefreshSideColor?.Invoke();
    }

    private void OnSideColorChanged(string color)
    {
        Setting.Info.SideColor = color;
        PageAction.RefreshSideColor?.Invoke();
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
        });
    }

    private void OnReset()
    {
        Setting.UserSetting.Info = null;
        PageAction.RefreshHeadColor?.Invoke();
        PageAction.RefreshSideColor?.Invoke();
        SetData(Setting.Info);
    }
}