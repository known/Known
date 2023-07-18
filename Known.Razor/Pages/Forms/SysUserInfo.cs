namespace Known.Razor.Pages.Forms;

class SysUserInfo : BaseComponent
{
    [Parameter] public UserInfo User { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div("user-info", attr =>
        {
            builder.Div("avatar", attr => builder.Img(attr => attr.Src($"_content/Known.Razor{User?.AvatarUrl}")));
            BuildUserItem(builder, "fa fa-user", $"{User?.Name}({User?.UserName})");
            BuildUserItem(builder, "fa fa-phone-square", User?.Phone);
            BuildUserItem(builder, "fa fa-tablet", User?.Mobile);
            BuildUserItem(builder, "fa fa-envelope-o", User?.Email);
            BuildUserItem(builder, "fa fa-users", User?.Role);
            BuildUserItem(builder, "fa fa-vcard-o", User?.Note);
        });
    }

    private static void BuildUserItem(RenderTreeBuilder builder, string icon, string value)
    {
        builder.Div("form-item", attr => builder.IconName(icon, value));
    }
}