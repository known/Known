namespace Known.Pages;

[Route("/profile/password")]
public class PasswordEditForm : BaseForm<PwdFormInfo>
{
    protected override async Task OnInitFormAsync()
    {
        Model = new FormModel<PwdFormInfo>(Context)
        {
            LabelSpan = 4,
            WrapperSpan = 6,
            Data = new PwdFormInfo()
        };

        await base.OnInitFormAsync();
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
                    UI.Button(builder, Language["Button.ConfirmUpdate"], this.Callback<MouseEventArgs>(OnSaveAsync), "primary");
                });
            });
        });
    }

    private async void OnSaveAsync(MouseEventArgs arg)
    {
        if (!Model.Validate())
            return;

        var result = await Platform.Auth.UpdatePasswordAsync(Model.Data);
        UI.Result(result);
    }
}