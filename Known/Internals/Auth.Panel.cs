namespace Known.Internals;

class AuthPanel : BaseComponent
{
    private readonly ActiveInfo Data = new();

    [Parameter] public RenderFragment ChildContent { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            var info = await Admin.GetProductAsync();
            Data.ProductId = info?.ProductId;
            Data.ProductKey = info?.ProductKey;
            await StateChangedAsync();
        }
    }

    protected override void BuildRender(RenderTreeBuilder builder)
    {
        if (!UIConfig.IsAuth)
            BuildAuthorize(builder);
        else
            ChildContent?.Invoke(builder);
    }

    private void BuildAuthorize(RenderTreeBuilder builder)
    {
        builder.Component<SysActive>()
               .Set(c => c.AuthStatus, UIConfig.AuthStatus)
               .Set(c => c.Data, Data)
               .Set(c => c.OnCheck, result =>
               {
                   UIConfig.IsAuth = result.IsValid;
                   UIConfig.AuthStatus = result.Message;
                   StateChanged();
               })
               .Build();
    }
}