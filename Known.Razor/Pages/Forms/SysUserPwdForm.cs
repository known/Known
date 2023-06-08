namespace Known.Razor.Pages.Forms;

class SysUserPwdForm : BaseForm<PwdFormInfo>
{
    public SysUserPwdForm()
    {
        Style = "";
    }

    protected override void OnInitialized()
    {
        Model = CurrentUser;
    }

    protected override void BuildFields(FieldBuilder<PwdFormInfo> builder)
    {
        builder.Field<Password>("原密码", nameof(PwdFormInfo.OldPwd), true).Build();
        builder.Field<Password>("新密码", nameof(PwdFormInfo.NewPwd), true).Build();
        builder.Field<Password>("确认密码", nameof(PwdFormInfo.NewPwd1), true).Build();
    }

    protected override void BuildButtons(RenderTreeBuilder builder)
    {
        builder.Button(FormButton.EditOK, Callback(e => OnSave()));
    }

    private void OnSave()
    {
        SubmitAsync(data =>
        {
            var info = new PwdFormInfo
            {
                OldPwd = data.OldPwd,
                NewPwd = data.NewPwd,
                NewPwd1 = data.NewPwd1,
            };
            return Platform.User.UpdatePasswordAsync(info);
        });
    }
}