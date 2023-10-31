namespace KnownAntDesign.Web.Pages;

[Route("/")]
public class Index : Known.Pages.Index
{
    protected override void BuildLogin(RenderTreeBuilder builder)
    {
        builder.Component<Login>().Set(c => c.OnLogin, OnLogin).Build();
    }

    protected override void BuildAdmin(RenderTreeBuilder builder)
    {
        builder.Component<Admin>().Build();
    }

    //第三方登录设置当前用户
    //protected override async Task OnAfterRenderAsync(bool firstRender)
    //{
    //    if (firstRender)
    //        await SetCurrentUserAsync(CurrentUser);
    //    await base.OnAfterRenderAsync(firstRender);
    //}

    //第三方登录获取当前用户
    //protected override async Task<UserInfo> GetThirdUserAsync()
    //{
    //    //var third = ThirdApi.GetUser();
    //    var third = new { UserName = "admin" };
    //    var user = await Platform.GetUserAsync(third.UserName);
    //    return user;
    //}
}