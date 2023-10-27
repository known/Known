namespace Known.Pages;

class SysSystemInfo : PageComponent
{
    private SystemInfo info;

    protected override async Task InitPageAsync()
    {
        info = await Platform.System.GetSystemAsync();
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var user = CurrentUser;
        var status = Config.AuthStatus;
        var style = string.IsNullOrWhiteSpace(status) ? "success" : "danger";
        if (string.IsNullOrWhiteSpace(status))
            status = "已授权";
        builder.Div("ss-form ss-system", attr =>
        {
            var label = Config.IsPlatform ? "租户名称：" : "企业名称：";
            builder.Field<KText>(label, "").Value($"{info?.CompNo}-{info?.CompName}").ReadOnly(true).Build();
            builder.Field<KText>("系统名称：", "").Value(info?.AppName).ReadOnly(true)
                   .Set(f => f.IsEdit, true)
                   .Set(f => f.OnSave, async value =>
                   {
                       info.AppName = value;
                       await Platform.System.SaveSystemAsync(info);
                       PageAction.RefreshAppName?.Invoke(value);
                   })
                   .Build();
            builder.Field<KText>("系统版本：", "").Value(Config.Version.AppVersion).ReadOnly(true).Build();
            builder.Field<KText>("软件版本：", "").Value(Config.Version.SoftVersion).ReadOnly(true).Build();
            builder.Field<KText>("框架版本：", "").Value(Config.Version.FrameVersion).ReadOnly(true).Build();
            if (!Config.IsPlatform && Config.IsProductKey)
            {
                builder.Field<KText>("产品ID：", "").Value(info?.ProductId).ReadOnly(true).Build();
                builder.Field<KText>("产品密钥：", "ProductKey").Value(info?.ProductKey).ReadOnly(true)
                       .Set(f => f.IsEdit, true)
                       .Set(f => f.OnSave, async value =>
                       {
                           info.ProductKey = value;
                           await Platform.System.SaveKeyAsync(info);
                           StateChanged();
                       })
                       .Build();
                builder.Field<KText>("授权信息：", "").InputTemplate(b => b.Span($"text bold {style}", status)).Build();
            }
            builder.Field<KText>("版权信息：", "").Style("ss-copyright").Value(info?.Copyright ?? Config.Copyright).ReadOnly(true)
                   .Set(f => f.IsEdit, Config.IsEditCopyright && user.IsSystemAdmin())
                   .Set(f => f.OnSave, async value =>
                   {
                       info.Copyright = value;
                       await Platform.System.SaveSystemConfigAsync(info);
                       StateChanged();
                   })
                   .Build();
            builder.Field<KTextArea>("软件许可：", "").Style("ss-terms").Value(info?.SoftTerms ?? Config.SoftTerms).ReadOnly(true)
                   .Set(f => f.IsEdit, Config.IsEditCopyright && user.IsSystemAdmin())
                   .Set(f => f.OnSave, async value =>
                   {
                       info.SoftTerms = value;
                       await Platform.System.SaveSystemConfigAsync(info);
                       StateChanged();
                   })
                   .Build();
        });
    }
}