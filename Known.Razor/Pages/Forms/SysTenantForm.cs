namespace Known.Razor.Pages.Forms;

[Dialog(600, 340)]
class SysTenantForm : BaseForm<SysTenant>
{
    protected override void BuildFields(FieldBuilder<SysTenant> builder)
    {
        builder.Hidden(f => f.Id);
        builder.Table(table =>
        {
            table.ColGroup(20, 30, 20, 30);
            table.Tr(attr =>
            {
                table.Field<Text>(f => f.Code).Build();
                table.Field<Text>(f => f.Name).Build();
            });
            table.Tr(attr => table.Field<CheckBox>(f => f.Enabled).ColSpan(3).Set(f => f.Switch, true).Build());
            table.Tr(attr =>
            {
                table.Field<Number>(f => f.UserCount).Build();
                table.Field<Number>(f => f.BillCount).Build();
            });
            table.Tr(attr => table.Field<TextArea>(f => f.Note).ColSpan(3).Build());
        });
    }

    protected override void BuildButtons(RenderTreeBuilder builder)
    {
        builder.Button(FormButton.Save, Callback(OnSave), !ReadOnly);
        base.BuildButtons(builder);
    }

    private void OnSave() => SubmitAsync(Platform.System.SaveTenantAsync);
}