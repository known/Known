namespace Known.AntBlazor.Components;

public class AntSelect : Select<string, CodeInfo>
{
    [Parameter] public int ColSpan { get; set; }
    [Parameter] public string Label { get; set; }
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
        {
            base.BuildRenderTree(builder);
            return;
        }
    }
}