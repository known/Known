namespace Known.Razor.Pages.Forms;

class SysUserInfo : BaseComponent
{
    public override void Refresh()
    {
        StateChanged();
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var user = CurrentUser;
        builder.Div("user-info", attr =>
        {
            builder.Div("avatar", attr => builder.Img(attr => attr.Src($"_content/Known.Razor{user?.AvatarUrl}")));
            builder.Div("userInfo", attr =>
            {
                BuildUserItem(builder, "fa fa-user", $"{user?.Name}({user?.UserName})");
                BuildUserItem(builder, "fa fa-phone-square", user?.Phone);
                BuildUserItem(builder, "fa fa-tablet", user?.Mobile);
                BuildUserItem(builder, "fa fa-envelope-o", user?.Email);
                BuildUserItem(builder, "fa fa-users", user?.Role);
                BuildUserItem(builder, "fa fa-vcard-o", user?.Note);
            });
        });
    }

    private static void BuildUserItem(RenderTreeBuilder builder, string icon, string value)
    {
        builder.Div("form-item", attr => builder.IconName(icon, value));
    }
}