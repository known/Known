namespace Known.Pages;

/// <summary>
/// 用户修改密码表单组件类。
/// </summary>
[Route("/profile/password")]
public class PasswordEditForm : BaseForm<PwdFormInfo>
{
    /// <inheritdoc />
    protected override async Task OnInitFormAsync()
    {
        await base.OnInitFormAsync();
        Model = new FormModel<PwdFormInfo>(this, true) { Data = new PwdFormInfo() };
    }

    /// <inheritdoc />
    protected override void BuildForm(RenderTreeBuilder builder)
    {
        builder.FormPage(() =>
        {
            builder.Div("kui-user-form", () =>
            {
                base.BuildForm(builder);
                builder.FormButton(() =>
                {
                    builder.Button(Language.ConfirmUpdate, this.Callback<MouseEventArgs>(OnSaveAsync));
                });
            });
        });
    }

    private async Task OnSaveAsync(MouseEventArgs arg)
    {
        if (!Model.Validate())
            return;

        var result = await Admin.UpdatePasswordAsync(Model.Data);
        UI.Result(result);
    }
}