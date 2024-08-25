namespace Known.Pages;

[StreamRendering]
[Route("/profile/user")]
public class UserEditForm : BaseEditForm<SysUser>
{
    private IUserService Service;
    [CascadingParameter] private SysUserProfile Parent { get; set; }

    protected override async Task OnInitFormAsync()
    {
        await base.OnInitFormAsync();
        Service = await CreateServiceAsync<IUserService>();
        var data = Parent?.User;
        data ??= await Service.GetUserAsync(CurrentUser.Id);
        Model = new FormModel<SysUser>(this)
        {
            Info = new FormInfo { LabelSpan = 4, WrapperSpan = 8 },
            Data = data
        };
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
        return Service.UpdateUserAsync(model);
    }

    protected override void OnSuccess(Result result)
    {
        var entity = result.DataAs<SysUser>();
        if (entity != null)
        {
            var user = CurrentUser;
            user.Name = entity.Name;
            user.EnglishName = entity.EnglishName;
            user.Gender = entity.Gender;
            user.Phone = entity.Phone;
            user.Mobile = entity.Mobile;
            user.Email = entity.Email;
            user.Note = entity.Note;
            App?.SetCurrentUserAsync(user);
        }
        Navigation.Refresh(true);
    }
}