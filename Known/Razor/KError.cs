namespace Known.Razor;

public class KError : BaseComponent
{
    [Parameter] public string Code { get; set; }
    [Parameter] public string Title { get; set; }
    [Parameter] public string Message { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var title = Title;
        var message = Message;
        switch (Code)
        {
            case "403":
                title = Language.Error403Title;
                message = Language.Error403Content;
                break;
            case "404":
                title = Language.Error404Title;
                message = Language.Error404Content;
                break;
            case "500":
                title = Language.Error500Title;
                message = Language.Error500Content;
                break;
        }

        builder.Div("error-box", attr =>
        {
            builder.H1(Code);
            builder.H3(title);
            builder.Div("", message);
        });
    }
}