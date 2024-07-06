namespace Known.Components;

public class KAuthPanel : BaseComponent
{
    [Parameter] public RenderFragment ChildContent { get; set; }

    protected override void BuildRender(RenderTreeBuilder builder)
    {
        if (!Config.IsAuth)
            BuildAuthorize(builder);
        else
            ChildContent?.Invoke(builder);
    }

    private void BuildAuthorize(RenderTreeBuilder builder)
    {
        builder.Component<SysActive>()
               .Set(c => c.OnCheck, isCheck =>
               {
                   Config.IsAuth = isCheck;
                   StateChanged();
               })
               .Build();
    }
}