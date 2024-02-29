namespace Known.AntBlazor.Components;

public class AntNumber<TValue> : InputNumber<TValue>, IAntField
{
    [CascadingParameter] private IAntForm AntForm { get; set; }

    public Type ValueType => typeof(TValue);
    [Parameter] public int Span { get; set; }
    [Parameter] public string Label { get; set; }
    [Parameter] public bool Required { get; set; }

    protected override void OnInitialized()
    {
        if (AntForm != null)
            Disabled = AntForm.IsView;
        base.OnInitialized();
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (string.IsNullOrWhiteSpace(Label))
            base.BuildRenderTree(builder);
        else
            builder.FormItem(this, b => base.BuildRenderTree(b));
    }
}