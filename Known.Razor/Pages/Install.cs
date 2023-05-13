namespace Known.Razor.Pages;

public class Install : FormComponent
{
    public Install()
    {
        FormStyle = "inline si-form";
    }

    protected override bool IsTable => false;

    [Parameter] public Action<CheckInfo> OnInstall { get; set; }

    protected override void OnInitialized()
    {
        Model = Context.Check.Install;
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div("install", attr => BuildPage(builder));
    }

    protected override void BuildFields(RenderTreeBuilder builder)
    {
        builder.Div("si-title", $"欢迎使用{Config.AppId}");
        builder.Field<Text>("企业编码：", nameof(InstallInfo.CompNo), true).Build();
        builder.Field<Text>("企业名称：", nameof(InstallInfo.CompName), true)
               .Set(f => f.OnValueChanged, OnCompNameChanged)
               .Build();
        builder.Field<Text>("系统名称：", nameof(InstallInfo.AppName), true).Build();
        BuildProduct(builder);
        builder.Field<Text>("管理员账号：", nameof(InstallInfo.UserName)).ReadOnly(true).Build();
        builder.Field<Password>("管理员密码：", nameof(InstallInfo.Password), true).Build();
        builder.Field<Password>("确认密码：", nameof(InstallInfo.Password1), true).Build();
    }

    protected override void BuildButtons(RenderTreeBuilder builder)
    {
        builder.Button("开始使用", Callback(e => OnStart()), "si-button");
    }

    private void BuildProduct(RenderTreeBuilder builder)
    {
        builder.Field<Text>("产品ID：", nameof(InstallInfo.ProductId)).ReadOnly(true).Build();
        builder.Div("form-item", attr =>
        {
            builder.Label("form-label required", attr =>
            {
                attr.For(nameof(InstallInfo.ProductKey));
                builder.Text("产品密钥：");
            });
            builder.Field<Text>(nameof(InstallInfo.ProductKey), true).Build();
            builder.Span("fa fa-refresh link", attr =>
            {
                attr.Title("免费获取").OnClick(Callback(OnUpdateKey));
            });
        });
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
        var fCompNo = form.Fields[nameof(InstallInfo.CompNo)];
        var fCompName = form.Fields[nameof(InstallInfo.CompName)];
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
        form.Fields[nameof(InstallInfo.ProductKey)].SetValue(result.Data);
    }

    private void OnCompNameChanged(FieldContext context)
    {
        //var name = context.FieldValue.ToString();
        //var compNo = Utils.GetPinyin(name);
        //context.Fields["CompNo"].SetValue(compNo);
        //context.Fields["AppName"].SetValue($"{name}{Config.AppName}");
    }
}