namespace Known.Pages;

[StreamRendering]
[Route("/profile/password")]
public class PasswordEditForm : BaseForm<PwdFormInfo>
{
    private IAuthService Service;

    protected override async Task OnInitFormAsync()
    {
        await base.OnInitFormAsync();
        Service = await CreateServiceAsync<IAuthService>();
        Model = new FormModel<PwdFormInfo>(this, true)
        {
            Info = new FormInfo { LabelSpan = 4, WrapperSpan = 6 },
            Data = new PwdFormInfo()
        };
    }

    protected override void BuildForm(RenderTreeBuilder builder)
    {
        builder.FormPage(() =>
        {
            builder.Div("form-password", () =>
            {
                base.BuildForm(builder);
                builder.FormButton(() =>
                {
                    builder.Button(Language["Button.ConfirmUpdate"], this.Callback<MouseEventArgs>(OnSaveAsync));
                });
            });
        });
    }

    private async void OnSaveAsync(MouseEventArgs arg)
    {
        if (!Model.Validate())
            return;

        var result = await Service.UpdatePasswordAsync(Model.Data);
        UI.Result(result);
    }
}