namespace Known.Pages;

/// <summary>
/// 用户修改密码表单组件类。
/// </summary>
[Route("/profile/password")]
public class PasswordEditForm : BaseComponent
{
    /// <inheritdoc />
    protected override void BuildRender(RenderTreeBuilder builder)
    {
        builder.FormPage(() =>
        {
            builder.Div("kui-user-form", () =>
            {
                builder.Component<PasswordForm>()
                       .Set(c => c.OnSave, OnSaveAsync)
                       .Build();
            });
        });
    }

    private async Task OnSaveAsync(PwdFormInfo info)
    {
        var result = await Admin.UpdatePasswordAsync(info);
        UI.Result(result);
    }
}