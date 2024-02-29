namespace Known.AntBlazor.Components;

public class AntSelect : Select<string, CodeInfo>, IAntField
{
    public Type ValueType => typeof(string);
    [Parameter] public int Span { get; set; }
    [Parameter] public string Label { get; set; }
    [Parameter] public bool Required { get; set; }
    [Parameter] public List<CodeInfo> Codes { get; set; }

	protected override void OnInitialized()
	{
        ValueName = nameof(CodeInfo.Code);
		LabelName = nameof(CodeInfo.Name);
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