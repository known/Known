namespace Known.Razor.Pages.Forms;

[Dialog(500, 340)]
class SysOrgForm : BaseForm<SysOrganization>
{
    protected override void BuildFields(FieldBuilder<SysOrganization> builder)
    {
        builder.Hidden(f => f.Id);
        builder.Hidden(f => f.ParentId);
        builder.Table(table =>
        {
            table.ColGroup(100, null);
            table.Tr(attr => builder.Field<Text>(f => f.ParentName).ReadOnly(true).Build());
            table.Tr(attr => builder.Field<Text>(f => f.Code).Build());
            table.Tr(attr => builder.Field<Text>(f => f.Name).Build());
            table.Tr(attr => builder.Field<TextArea>(f => f.Note).Build());
        });
    }

    protected override void BuildButtons(RenderTreeBuilder builder)
    {
        builder.Button(FormButton.Save, Callback(OnSave), !ReadOnly);
        base.BuildButtons(builder);
    }

    private void OnSave() => SubmitAsync(Platform.Company.SaveOrganizationAsync);
}