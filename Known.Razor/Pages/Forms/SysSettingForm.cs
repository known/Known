namespace Known.Razor.Pages.Forms;

class SysSettingForm : BaseForm<SettingInfo>
{
    private bool isEdit = false;
    private readonly CodeInfo[] themes = new CodeInfo[]
    {
        new CodeInfo("Default", "默认")
    };
    private readonly string sizes = string.Join(",", PagingCriteria.PageSizes);

    public SysSettingForm()
    {
        Style = "";
    }

    protected override void OnInitialized()
    {
        Model = Setting.Info;
    }

    protected override void BuildFields(FieldBuilder<SettingInfo> builder)
    {
        builder.Field<Select>("系统主题", nameof(SettingInfo.Theme)).ReadOnly(!isEdit).Set(f => f.Items, themes).Build();
        builder.Field<CheckBox>("标签页", nameof(SettingInfo.MultiTab)).ReadOnly(!isEdit).Build();
        builder.Field<Select>("表格每页显示数量", nameof(SettingInfo.PageSize)).ReadOnly(!isEdit).Set(f => f.Codes, sizes).Build();
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