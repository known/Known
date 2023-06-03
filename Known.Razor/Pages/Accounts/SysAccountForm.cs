namespace Known.Razor.Pages.Accounts;

class SysAccountForm : BaseForm<SysUser>
{
    private bool isEdit = false;

    protected override void OnInitialized()
    {
        Model = CurrentUser;
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        BuildForm(builder);
    }

    protected override void BuildFields(FieldBuilder<SysUser> builder)
    {
        builder.Div("avatar", attr => builder.Builder.Icon("fa fa-user"));
        builder.Hidden(f => f.Id);
        builder.Field<Text>(f => f.UserName).ReadOnly(true).Build();
        builder.Field<Text>(f => f.Name).ReadOnly(!isEdit).Build();
        builder.Field<Text>(f => f.Phone).ReadOnly(!isEdit).Build();
        builder.Field<Text>(f => f.Mobile).ReadOnly(!isEdit).Build();
        builder.Field<Text>(f => f.Email).ReadOnly(!isEdit).Build();
        builder.Field<Text>(f => f.Role).ReadOnly(true).Build();
    }

    protected override void BuildButtons(RenderTreeBuilder builder)
    {
        if (!isEdit)
        {
            builder.Button(FormButton.Edit, Callback(e => isEdit = true));
        }
        else
        {
            builder.Button(FormButton.Save, Callback(OnSave));
            builder.Button(FormButton.Cancel, Callback(e => isEdit = false));
        }
    }

    private void OnSave()
    {
        SubmitAsync(Platform.User.UpdateUserAsync, result =>
        {
            var curUser = CurrentUser;
            var user = result.DataAs<SysUser>();
            if (curUser != null && user != null)
            {
                curUser.Name = user.Name;
                curUser.Phone = user.Phone;
                curUser.Mobile = user.Mobile;
                curUser.Email = user.Email;
            }
            isEdit = false;
            StateChanged();
        });
    }
}