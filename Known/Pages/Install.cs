namespace Known.Pages;

public class Install : KForm
{
    public Install()
    {
        Style = "box install animated fadeIn";
    }

    [Parameter] public Action<InstallInfo> OnInstall { get; set; }

    protected override void OnInitialized()
    {
        Model = Context.Install;
    }

    protected override void BuildFields(RenderTreeBuilder builder)
    {
        builder.Div("title", $"欢迎使用{Config.AppId}");
        builder.Field<KText>("企业编码：", nameof(InstallInfo.CompNo), true).Build();
        builder.Field<KText>("企业名称：", nameof(InstallInfo.CompName), true)
               .Set(f => f.OnValueChanged, OnCompNameChanged)
               .Build();
        builder.Field<KText>("系统名称：", nameof(InstallInfo.AppName), true).Build();
        BuildProduct(builder);
        builder.Field<KText>("管理员账号：", nameof(InstallInfo.UserName)).ReadOnly(true).Build();
        builder.Field<KPassword>("管理员密码：", nameof(InstallInfo.Password), true).Build();
        builder.Field<KPassword>("确认密码：", nameof(InstallInfo.Password1), true).Build();
        builder.Div("form-button", attr => BuildButton(builder));
    }

    protected override void BuildButtons(RenderTreeBuilder builder) { }

    private void BuildButton(RenderTreeBuilder builder)
    {
        builder.Button("开始使用", Callback(e => OnStart()), StyleType.Primary);
    }

    private void BuildProduct(RenderTreeBuilder builder)
    {
        if (!Config.IsProductKey)
            return;

        builder.Field<KText>("产品ID：", nameof(InstallInfo.ProductId)).ReadOnly(true).Build();
        builder.Div("form-item", attr =>
        {
            builder.Label("form-label required", attr =>
            {
                attr.For(nameof(InstallInfo.ProductKey));
                builder.Text("产品密钥：");
            });
            builder.Field<KText>(nameof(InstallInfo.ProductKey), true).Build();
            builder.Span("fa fa-refresh link", attr =>
            {
                attr.Title("免费获取").OnClick(Callback(OnUpdateKey));
            });
        });
    }

    private Task OnStart()
    {
        return SubmitAsync(data => Platform.System.SaveInstallAsync((InstallInfo)Model), result =>
        {
            if (result.IsValid)
                OnInstall?.Invoke(result.DataAs<InstallInfo>());
        });
    }

    private async Task OnUpdateKey()
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