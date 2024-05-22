namespace Known.Pages;

[Authorize]
[Route("/profile/user")]
public class UserEditForm : BaseEditForm<SysUser>
{
    [CascadingParameter] private SysUserProfile Parent { get; set; }

    protected override async Task OnInitFormAsync()
    {
        Data = Parent?.User;
        Data ??= await Platform.User.GetUserAsync(CurrentUser.Id);

        await base.OnInitFormAsync();
        Model.LabelSpan = 4;
        Model.WrapperSpan = 8;
        Model.AddRow().AddColumn(c => c.UserName, c => c.ReadOnly = true);
        Model.AddRow().AddColumn(c => c.Name);
        Model.AddRow().AddColumn(c => c.EnglishName);
        Model.AddRow().AddColumn(c => c.Gender, c => c.Type = FieldType.RadioList);
        Model.AddRow().AddColumn(c => c.Phone);
        Model.AddRow().AddColumn(c => c.Mobile);
        Model.AddRow().AddColumn(c => c.Email);
        Model.AddRow().AddColumn(c => c.Note, c => c.Type = FieldType.TextArea);
    }

    protected override void BuildFormContent(RenderTreeBuilder builder)
    {
        builder.FormPage(() =>
        {
            builder.Div("form-user", () =>
            {
                UI.BuildForm(builder, Model);
                builder.FormButton(() => BuildAction(builder));
            });
        });
    }

    protected override Task<Result> OnSaveAsync(SysUser model)
    {
        return Platform.Auth.UpdateUserAsync(model);
    }

    protected override void OnSuccess()
    {
        Parent?.UpdateProfileInfo();
    }
}