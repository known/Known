using Known.Razor.Pages.Forms;

namespace Known.Razor.Pages;

class SysAccount : PageComponent
{
    private readonly List<MenuItem> items = new()
    {
        new MenuItem("基本信息", "fa fa-user", typeof(SysAccountForm)),
        new MenuItem("修改密码", "fa fa-lock", typeof(SysUserPwdForm)),
        new MenuItem("系统设置", "fa fa-cog", typeof(SysSettingForm))
    };
    private string curItem;
    private Type currType;

    protected override void OnInitialized()
    {
        curItem = items[0].Id;
        currType = items[0].ComType;
    }

    protected override void BuildPage(RenderTreeBuilder builder)
    {
        builder.Div("ss-form", attr =>
        {
            builder.Component<Tab>()
                   .Set(c => c.Position, "left")
                   .Set(c => c.CurItem, curItem)
                   .Set(c => c.Items, items)
                   .Set(c => c.OnChanged, OnTabChanged)
                   .Build();
            builder.DynamicComponent(currType);
        });
    }

    private void OnTabChanged(MenuItem item)
    {
        curItem = item.Id;
        currType = item.ComType;
        StateChanged();
    }
}