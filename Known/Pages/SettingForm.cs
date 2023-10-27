namespace Known.Pages;

class SettingForm : BaseForm<SettingInfo>
{
    private readonly string sizes = string.Join(",", PagingCriteria.PageSizes);

    public SettingForm()
    {
        Style = "setting";
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
        builder.Field<LayoutField>("布局", nameof(SettingInfo.Layout))
               .Set(f => f.Style, "layout")
               .Set(f => f.ValueChanged, OnLayoutChanged)
               .Build();
        builder.Field<KInput>("主题色", nameof(SettingInfo.ThemeColor))
               .Set(f => f.Type, InputType.Color)
               .Set(f => f.ValueChanged, OnThemeColorChanged)
               .Build();
        builder.Field<KInput>("侧栏色", nameof(SettingInfo.SiderColor))
               .Set(f => f.Type, InputType.Color)
               .Set(f => f.ValueChanged, OnSiderColorChanged)
               .Build();
        builder.Field<KCheckBox>("随机色", nameof(SettingInfo.RandomColor)).Set(f => f.Switch, true).Build();
        builder.Field<KCheckBox>("标签页", nameof(SettingInfo.MultiTab))
               .Set(f => f.Switch, true)
               .Set(f => f.ValueChanged, OnMultiTabChanged)
               .Build();
        builder.Field<KSelect>("每页大小", nameof(SettingInfo.PageSize)).Set(f => f.Codes, sizes).Build();
    }

    protected override void BuildButtons(RenderTreeBuilder builder)
    {
        builder.Button(FormButton.Save, Callback(OnSave));
        builder.Button(FormButton.Reset, Callback(OnReset));
    }

    private void OnLayoutChanged(string value)
    {
        Setting.Info.Layout = value;
        OnThemeChanged();
    }

    private void OnThemeColorChanged(string value)
    {
        Setting.Info.ThemeColor = value;
        OnThemeChanged();
    }

    private void OnSiderColorChanged(string value)
    {
        Setting.Info.SiderColor = value;
        OnThemeChanged();
    }

    private void OnMultiTabChanged(string value)
    {
        Setting.Info.MultiTab = Utils.ConvertTo<bool>(value);
        OnThemeChanged();
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
            Setting.Info = Utils.FromJson<SettingInfo>(info.Data);
            return Platform.User.SaveSettingAsync(info);
        });
    }

    private void OnReset()
    {
        Setting.Info = SettingInfo.Default;
        OnThemeChanged();
        SetData(SettingInfo.Default);
    }

    private static void OnThemeChanged() => PageAction.RefreshTheme?.Invoke();
}

class LayoutField : Field
{
    protected override void BuildInput(RenderTreeBuilder builder)
    {
        BuildLayout(builder, "");
        BuildLayout(builder, "layout-tl");
    }

    private void BuildLayout(RenderTreeBuilder builder, string layout)
    {
        var value = Value ?? "";
        var style = CssBuilder.Default(layout).AddClass("checked", value == layout).Build();
        builder.Div("slayout", attr =>
        {
            attr.OnClick(Callback(e => OnItemClick(layout)));
            builder.Component<FLayout>().Set(f => f.Style, style).Build();
        });
    }

    private void OnItemClick(string value)
    {
        Value = value;
        OnValueChange();
    }
}

class FLayout : KLayout
{
    public FLayout()
    {
        IsDemo = true;
    }
}