namespace Known.AntBlazor;

class ModalContent : ComponentBase
{
    private bool showLoading = true;

    [Parameter] public RenderFragment Body { get; set; }

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
            builder.Component<ModalBody>().Set(c => c.Body, Body).Build();
        }
    }
}

class ModalBody : ComponentBase
{
    private bool spinning = true;

    [Parameter] public RenderFragment Body { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        spinning = false;
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Component<Spin>()
               .Set(c => c.Spinning, spinning)
               .Set(c => c.ChildContent, b => Body?.Invoke(b))
               .Build();
    }
}