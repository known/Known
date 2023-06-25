using Known.Razor.Pages;

namespace Known.Test;

[Route("/")]
public class Index : Razor.Pages.Index
{
    protected override void BuildLogin(RenderTreeBuilder builder)
    {
        builder.Component<Login>()
               .Set(c => c.IsCaptcha, true)
               .Set(c => c.OnLogin, OnLogin)
               .Build();
    }
}