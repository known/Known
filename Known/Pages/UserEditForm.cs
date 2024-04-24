﻿namespace Known.Pages;

[Route("/profile/user")]
public class UserEditForm : BaseEditForm<SysUser>
{
    [CascadingParameter] private SysUserProfile Parent { get; set; }

    protected override async Task OnInitFormAsync()
    {
        Model = new FormModel<SysUser>(Context, false)
        {
            LabelSpan = 4,
            WrapperSpan = 8,
            IsView = true,
            Data = Parent?.User
        };
        if (Model.Data == null)
        {
            Model.Data = await Platform.User.GetUserAsync(CurrentUser.Id);
        }
        Model.AddRow().AddColumn(c => c.UserName, c => c.ReadOnly = true);
        Model.AddRow().AddColumn(c => c.Name);
        Model.AddRow().AddColumn(c => c.EnglishName);
        Model.AddRow().AddColumn(c => c.Gender, c => c.Type = FieldType.RadioList);
        Model.AddRow().AddColumn(c => c.Phone);
        Model.AddRow().AddColumn(c => c.Mobile);
        Model.AddRow().AddColumn(c => c.Email);
        Model.AddRow().AddColumn(c => c.Note, c => c.Type = FieldType.TextArea);

        await base.OnInitFormAsync();
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