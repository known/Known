namespace Known.Internals;

class KModalBody : BaseComponent
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

class KModalFooter : BaseComponent
{
    [Parameter] public bool Closable { get; set; }
    [Parameter] public RenderFragment Left { get; set; }
    [Parameter] public List<ActionInfo> Actions { get; set; }
    [Parameter] public Func<Task> OnOk { get; set; }
    [Parameter] public Func<Task> OnCancel { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.FormAction(() =>
        {
            if (Actions != null && Actions.Count > 0)
            {
                foreach (var action in Actions)
                {
                    builder.Button(action);
                }
            }
            builder.Button(Language.OK, this.Callback<MouseEventArgs>(e => OnOk?.Invoke()));
            if (Closable)
                builder.Button(Language.Cancel, this.Callback<MouseEventArgs>(e => OnCancel?.Invoke()), "default");
        }, Left);
    }
}