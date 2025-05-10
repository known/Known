namespace Known.Components;

/// <summary>
/// 授权组件面板类。
/// </summary>
public class AuthComponent : ComponentBase
{
    private ActiveInfo info = new();
    private IAuthComponent Auth;

    /// <summary>
    /// 取得或设置组件实例。
    /// </summary>
    [Parameter] public BaseComponent Component { get; set; }

    /// <summary>
    /// 取得或设置组件子内容。
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        base.OnInitialized();
        Auth = Component as IAuthComponent;
        if (Auth != null)
        {
            info = Auth.ValidateAuth() ?? new ActiveInfo();
            info.Type = ActiveType.Component;
        }
    }

    /// <inheritdoc />
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (Auth == null)
        {
            ChildContent?.Invoke(builder);
            return;
        }

        if (!info.IsValid)
            BuildAuthorize(builder);
        else
            ChildContent?.Invoke(builder);
    }

    private void BuildAuthorize(RenderTreeBuilder builder)
    {
        builder.Component<SysActive>()
               .Set(c => c.AuthStatus, info.Message)
               .Set(c => c.Data, info)
               .Set(c => c.OnCheck, result =>
               {
                   info.IsValid = result.IsValid;
                   info.Message = result.Message;
                   StateHasChanged();
               })
               .Build();
    }
}