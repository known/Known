namespace WebSite.Docus.Inputs.Passwords;

class Password1 : BaseComponent
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Field<Password>("密码1：", "Password1").Build();
        builder.Field<Password>("密码2：", "Password2").Set(f => f.Placeholder, "密码").Build();
    }
}