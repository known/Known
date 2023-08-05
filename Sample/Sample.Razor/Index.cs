namespace Sample.Razor;

public class Index : Known.Razor.Pages.Index
{
    protected override void BuildLogin(RenderTreeBuilder builder)
    {
        builder.Component<Login>().Set(c => c.OnLogin, OnLogin).Build();
    }
}