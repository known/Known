namespace Known.Razor.Components.Fields;

public class Hidden : Field
{
    protected override void BuildRenderTree(RenderTreeBuilder builder) => builder.Input(attr => attr.Type("hidden").Name(Id).Value(Value));
}

public class File : Field
{
    [Parameter] public string Placeholder { get; set; }
    [Parameter] public bool Multiple { get; set; }
    [Parameter] public Action<InputFileChangeEventArgs> OnFileChanged { get; set; }

    protected override void BuildChildContent(RenderTreeBuilder builder)
    {
        builder.Input(attr =>
        {
            attr.Type("file").Id(Id).Name(Id).Value(Value).Placeholder(Placeholder)
                .Add("multiple", Multiple)
                .Add("onchange", Callback(OnFileChanged));
        });
    }
}