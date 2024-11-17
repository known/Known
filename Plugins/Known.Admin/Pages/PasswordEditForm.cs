namespace Known.Pages;

/// <summary>
/// 用户修改密码表单组件类。
/// </summary>
[StreamRendering]
[Route("/profile/password")]
public class PasswordEditForm : BaseForm<PwdFormInfo>
{
    private IAuthService Service;

    /// <summary>
    /// 异步初始化表单。
    /// </summary>
    /// <returns></returns>
    protected override async Task OnInitFormAsync()
    {
        await base.OnInitFormAsync();
        Service= await CreateServiceAsync<IAuthService>();
        Model = new FormModel<PwdFormInfo>(this, true) { Data = new PwdFormInfo() };
    }

    /// <summary>
    /// 构建表单内容。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    protected override void BuildForm(RenderTreeBuilder builder)
    {
        builder.FormPage(() =>
        {
            builder.Div("kui-user-form", () =>
            {
                base.BuildForm(builder);
                builder.FormButton(() =>
                {
                    builder.Button(Language["Button.ConfirmUpdate"], this.Callback<MouseEventArgs>(OnSaveAsync));
                });
            });
        });
    }

    private async Task OnSaveAsync(MouseEventArgs arg)
    {
        if (!Model.Validate())
            return;

        var result = await Service.UpdatePasswordAsync(Model.Data);
        UI.Result(result);
    }
}