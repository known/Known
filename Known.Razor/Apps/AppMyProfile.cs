namespace Known.Razor.Apps;

public class AppMyProfile : FormComponent
{
    public AppMyProfile()
    {
        FormStyle = "";
        ButtonStyle = "";
    }

    protected override void OnInitialized() => Model = CurrentUser;

    protected override void BuildFields(RenderTreeBuilder builder)
    {
        builder.Hidden(nameof(UserInfo.Id));
        builder.Field<Text>("账号", nameof(UserInfo.UserName)).Enabled(false).Build();
        builder.Field<Text>("姓名", nameof(UserInfo.Name), true).Build();
        builder.Field<Text>("电话", nameof(UserInfo.Mobile)).Build();
        builder.Field<Text>("邮箱", nameof(UserInfo.Email)).Build();
    }

    protected override void BuildButtons(RenderTreeBuilder builder) => builder.Button(FormButton.Save, Callback(e => OnSave()));

    private void OnSave()
    {
        SubmitAsync(Platform.User.UpdateUserAsync, result =>
        {
            UI.Result(result, () =>
            {
                var data = result.DataAs<UserInfo>();
                var user = CurrentUser;
                user.Name = data.Name;
                user.Mobile = data.Mobile;
                user.Email = data.Email;
                Context.Back();
            });
        });
    }
}