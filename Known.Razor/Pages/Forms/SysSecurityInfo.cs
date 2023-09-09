namespace Known.Razor.Pages.Forms;

class SysSecurityInfo : PageComponent
{
    private SystemInfo info;

    protected override async Task InitPageAsync()
    {
        info = await Platform.System.GetSystemAsync();
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div("ss-form ss-system", attr =>
        {
            builder.Field<Text>("默认密码：", "").Value(info?.UserDefaultPwd).ReadOnly(true)
                   .Set(f => f.IsEdit, true)
                   .Set(f => f.OnSave, async value =>
                   {
                       info.UserDefaultPwd = value;
                       await Platform.System.SaveSystemAsync(info);
                   })
                   .Build();
        });
    }
}