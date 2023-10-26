namespace Known.Razor;

public class KHidden : Field
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Input(attr => attr.Type("hidden").Id(Id).Name(Id).Value(Value));
    }
}