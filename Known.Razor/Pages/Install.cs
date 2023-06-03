namespace Known.Razor.Pages;

public class Install : Form
{
    [Parameter] public Action<CheckInfo> OnInstall { get; set; }

    protected override void OnInitialized()
    {
        Style = "box install";
        IsInline = true;
        Model = Context.Check.Install;
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder) => BuildForm(builder);

    protected override void BuildFields(RenderTreeBuilder builder)
    {
        builder.H1($"欢迎使用{Config.AppId}");
        builder.Field<Text>("企业编码：", nameof(InstallInfo.CompNo), true).Build();
        builder.Field<Text>("企业名称：", nameof(InstallInfo.CompName), true)
               .Set(f => f.OnValueChanged, OnCompNameChanged)
               .Build();
        builder.Field<Text>("系统名称：", nameof(InstallInfo.AppName), true).Build();
        builder.Field<Text>("产品ID：", nameof(InstallInfo.ProductId)).ReadOnly(true).Build();
        builder.Field<Text>("产品密钥：", nameof(InstallInfo.ProductKey), true)
               .Set(c => c.Child, b =>
               {
                   b.Span("fa fa-refresh", attr =>
                   {
                       attr.Title("免费获取").OnClick(Callback(OnUpdateKey));
                   });
               })
               .Build();
        builder.Field<Text>("管理员账号：", nameof(InstallInfo.UserName)).ReadOnly(true).Build();
        builder.Field<Password>("管理员密码：", nameof(InstallInfo.Password), true).Build();
        builder.Field<Password>("确认密码：", nameof(InstallInfo.Password1), true).Build();
        builder.Button("开始使用", Callback(e => OnStart()), KRStyle.Primary);
    }

    private void OnStart()
    {
        SubmitAsync(data => Platform.System.SaveInstallAsync((InstallInfo)Model), result =>
        {
            if (result.IsValid)
                OnInstall?.Invoke(result.DataAs<CheckInfo>());
        });
    }

    private async void OnUpdateKey()
    {
        var fCompNo = Fields[nameof(InstallInfo.CompNo)];
        var fCompName = Fields[nameof(InstallInfo.CompName)];
        var vCompNo = fCompNo.Validate();
        var vCompName = fCompName.Validate();
        if (!vCompNo || !vCompName)
            return;

        var result = await Platform.System.UpdateKeyAsync((InstallInfo)Model);
        if (!result.IsValid)
        {
            UI.Alert(result.Message);
            return;
        }
        Fields[nameof(InstallInfo.ProductKey)].SetValue(result.Data);
    }

    private void OnCompNameChanged(FieldContext context)
    {
        //var name = context.FieldValue.ToString();
        //var compNo = Utils.GetPinyin(name);
        //context.Fields["CompNo"].SetValue(compNo);
        //context.Fields["AppName"].SetValue($"{name}{Config.AppName}");
    }
}