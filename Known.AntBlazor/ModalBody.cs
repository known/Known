namespace Known.AntBlazor;

class ModalBody : ComponentBase
{
    private bool showLoading = true;

    [Parameter] public RenderFragment Content { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            showLoading = false;
        }
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (!showLoading)
        {
            builder.Component<KLoading>().Set(c => c.ChildContent, Content).Build();
        }
    }
}

class ModalFooter : BaseComponent
{
    [Parameter] public Func<Task> OnOk { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.AntButton(Language?.OK, this.Callback<MouseEventArgs>(e => OnOk?.Invoke()));
    }
}