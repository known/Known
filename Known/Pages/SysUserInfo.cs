using Known.Extensions;
using Known.Razor;

namespace Known.Pages;

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
            builder.Div("avatar", attr => builder.Img(attr => attr.Src(user?.AvatarUrl)));
            builder.Div("userInfo", attr =>
            {
                BuildUserItem(builder, "fa fa-user", $"{user?.Name}({user?.UserName})");
                BuildUserItem(builder, "fa fa-phone-square", user?.Phone);
                BuildUserItem(builder, "fa fa-mobile", user?.Mobile);
                BuildUserItem(builder, "fa fa-envelope", user?.Email);
                BuildUserItem(builder, "fa fa-users", user?.Role);
                BuildUserItem(builder, "fa fa-comment", user?.Note);
            });
        });
    }

    private static void BuildUserItem(RenderTreeBuilder builder, string icon, string value)
    {
        builder.Div("form-item", attr => builder.IconName(icon, value));
    }
}