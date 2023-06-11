namespace Known.Razor.Pages;

class SysSystem : PageComponent
{
    private SystemInfo info;
    private readonly string Copyright = $"©2020-{DateTime.Now:yyyy} 普漫科技。保留所有权利。";
    private readonly string SoftTerms = "您对该软件的使用受您为获得该软件而签订的许可协议的条款和条件的约束。如果您是批量许可客户，则您对该软件的使用应受批量许可协议的约束。如果您未从普漫科技或其许可的分销商处获得该软件的有效许可，则不得使用该软件。";

    protected override async Task InitPageAsync()
    {
        info = await Platform.System.GetSystemAsync();
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var status = KRConfig.AuthStatus;
        var style = string.IsNullOrWhiteSpace(status) ? "success" : "danger";
        if (string.IsNullOrWhiteSpace(status))
            status = "已授权";
        builder.Div("ss-form ss-system", attr =>
        {
            builder.Field<Text>("企业名称：", "").Value($"{info?.CompNo}-{info?.CompName}").ReadOnly(true).Build();
            builder.Field<Text>("系统名称：", "").Value(info?.AppName).ReadOnly(true)
                   .Set(f => f.IsEdit, true)
                   .Set(f => f.OnSave, async value =>
                   {
                        info.AppName = value;
                        await Platform.System.SaveSystemAsync(info);
                   })
                   .Build();
            builder.Field<Text>("系统版本：", "").Value(Config.AppVersion).ReadOnly(true).Build();
            builder.Field<Text>("软件版本：", "").Value(Config.SysVersion).ReadOnly(true).Build();
            builder.Field<Text>("默认密码：", "").Value(info?.UserDefaultPwd).ReadOnly(true)
                   .Set(f => f.IsEdit, true)
                   .Set(f => f.OnSave, async value =>
                   {
                        info.UserDefaultPwd = value;
                        await Platform.System.SaveSystemAsync(info);
                   })
                   .Build();
            builder.Field<Text>("产品ID：", "").Value(info?.ProductId).ReadOnly(true).Build();
            builder.Field<Text>("产品密钥：", "ProductKey").Value(info?.ProductKey).ReadOnly(true)
                   .Set(f => f.IsEdit, true)
                   .Set(f => f.OnSave, async value =>
                   {
                        info.ProductKey = value;
                        await Platform.System.SaveSystemAsync(info);
                        StateChanged();
                   })
                   .Build();
            builder.Field<Text>("授权信息：", "").InputTemplate(b => b.Span($"text bold {style}", status)).Build();
            builder.Field<Text>("版权信息：", "").Value(Copyright).ReadOnly(true).Build();
            builder.Div("form-item ss-terms", attr =>
            {
                builder.Label("form-label", "软件许可：");
                builder.Div("text", SoftTerms);
            });
        });
    }
}