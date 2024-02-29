namespace Known.AntBlazor.Components;

public class AntRadioGroup : RadioGroup<string>, IAntField
{
    public Type ValueType => typeof(string);
    [Parameter] public int Span { get; set; }
    [Parameter] public string Label { get; set; }
    [Parameter] public bool Required { get; set; }
    [Parameter] public List<CodeInfo> Codes { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        //Fixed单选按钮组切换不刷新问题
        OnChange = EventCallback.Factory.Create<string>(this, value => StateHasChanged());
    }

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
        Options = Codes.ToRadioOptions();
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (string.IsNullOrWhiteSpace(Label))
            base.BuildRenderTree(builder);
        else
            builder.FormItem(this, b => base.BuildRenderTree(b));
    }
}