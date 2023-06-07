namespace Known.Razor.Pages.Forms;

[Dialog(700, 350)]
class SysUserForm : BaseForm<SysUser>
{
    private SysUser model;
    private UserAuthInfo auth;

    protected override async Task InitFormAsync()
    {
        model = TModel;
        auth = await Platform.User.GetUserAuthAsync(model?.Id);
    }

    protected override void BuildFields(FieldBuilder<SysUser> builder)
    {
        builder.Hidden(f => f.Id);
        builder.Table(table =>
        {
            table.ColGroup(100, null, 100, null);
            table.Tr(attr =>
            {
                builder.Field<Text>(f => f.UserName).Enabled(model.IsNew).Build();
                builder.Field<Text>(f => f.Name).Build();
            });
            table.Tr(attr =>
            {
                builder.Field<RadioList>(f => f.Gender).Set(f => f.Codes, "男,女").Build();
                builder.Field<Text>(f => f.Email).Build();
            });
            table.Tr(attr =>
            {
                builder.Field<Text>(f => f.Phone).Build();
                builder.Field<Text>(f => f.Mobile).Build();
            });
            table.Tr(attr =>
            {
                builder.Field<CheckList>("角色", "RoleId", true).ColSpan(3)
                       .Set(f => f.Value, auth?.RoleIds)
                       .Set(f => f.Items, auth?.Roles)
                       .Build();
            });
            table.Tr(attr =>
            {
                builder.Field<CheckList>("数据权限", nameof(SysUser.Data)).ColSpan(3)
                       .Set(f => f.Items, auth?.Datas)
                       .Build();
            });
        });
    }

    protected override void BuildButtons(RenderTreeBuilder builder)
    {
        builder.Button(FormButton.Save, Callback(OnSave), !ReadOnly);
        base.BuildButtons(builder);
    }

    private void OnSave() => SubmitAsync(Platform.User.SaveUserAsync);
}