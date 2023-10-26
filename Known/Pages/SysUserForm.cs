using Known.Extensions;
using Known.Razor;

namespace Known.Pages;

[Dialog(700, 350)]
public class SysUserForm : BaseForm<SysUser>
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
        builder.Hidden(f => f.OrgNo);
        builder.Table(table =>
        {
            table.ColGroup(100, null, 100, null);
            table.Tr(attr =>
            {
                table.Field<KText>(f => f.UserName).Enabled(model.IsNew).Build();
                table.Field<KText>(f => f.Name).Build();
            });
            table.Tr(attr =>
            {
                table.Field<KRadioList>(f => f.Gender).Set(f => f.Codes, "男,女").Build();
                table.Field<KInput>(f => f.Email).Set(f => f.Type, InputType.Email).Build();
            });
            table.Tr(attr =>
            {
                table.Field<KInput>(f => f.Phone).Set(f => f.Type, InputType.Tel).Build();
                table.Field<KInput>(f => f.Mobile).Set(f => f.Type, InputType.Tel).Build();
            });
            table.Tr(attr =>
            {
                table.Th("", "选项");
                table.Td(attr =>
                {
                    attr.ColSpan(3);
                    table.Div("inline", attr =>
                    {
                        table.Field<KCheckBox>("", nameof(SysUser.Enabled), true).IsInput(true).Set(f => f.Text, "启用").Build();
                        if (Config.IsPlatform && !CurrentUser.IsTenant)
                            table.Field<KCheckBox>("", nameof(SysUser.IsOperation)).IsInput(true).Set(f => f.Text, Constants.UTOperation).Build();
                        else
                            table.Hidden(f => f.IsOperation);
                    });
                });
            });
            table.Tr(attr =>
            {
                table.Field<KCheckList>("角色", "RoleId").ColSpan(3)
                     .Set(f => f.Value, auth?.RoleIds)
                     .Set(f => f.Items, auth?.Roles)
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