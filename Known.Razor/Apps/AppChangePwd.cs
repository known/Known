namespace Known.Razor.Apps;

public class AppChangePwd : FormComponent
{
    public AppChangePwd()
    {
        FormStyle = "";
        ButtonStyle = "";
    }

    protected override void BuildFields(RenderTreeBuilder builder)
    {
        builder.Field<Password>("旧密码", nameof(PwdFormInfo.OldPwd), true).Build();
        builder.Field<Password>("新密码", nameof(PwdFormInfo.NewPwd), true).Build();
        builder.Field<Password>("确认密码", nameof(PwdFormInfo.NewPwd1), true).Build();
    }

    protected override void BuildButtons(RenderTreeBuilder builder) => builder.Button(FormButton.EditOK, Callback(e => OnSave()));

    private void OnSave()
    {
        form?.Submit(async data =>
        {
            var info = new PwdFormInfo
            {
                OldPwd = data.OldPwd,
                NewPwd = data.NewPwd,
                NewPwd1 = data.NewPwd1,
            };
            var result = await Platform.User.UpdatePasswordAsync(info);
            UI.Result(result, () => Context.Back());
        });
    }
}