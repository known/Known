namespace Known.Components;

/// <summary>
/// 授权验证面板组件类。
/// </summary>
public class KAuthPanel : BaseComponent
{
    /// <summary>
    /// 取得或设置组件子内容模板。
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// 呈现授权组件内容。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
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