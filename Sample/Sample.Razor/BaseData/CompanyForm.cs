namespace Sample.Razor.BaseData;

class CompanyForm : WebForm<CompanyInfo>
{
    private bool isEdit = false;

    protected override async Task InitFormAsync()
    {
        Model = await Platform.Company.GetCompanyAsync<CompanyInfo>();
    }

    protected override void BuildFields(FieldBuilder<CompanyInfo> builder)
    {
        builder.Hidden(f => f.Code);
        builder.Table(table =>
        {
            table.ColGroup(15, 35, 15, 35);
            table.Tr(attr =>
            {
                table.Field<Text>(f => f.Name).ReadOnly(!isEdit).Build();
                table.Field<Text>(f => f.NameEn).ReadOnly(!isEdit).Build();
            });
            table.Tr(attr => table.Field<Text>(f => f.Address).ColSpan(3).ReadOnly(!isEdit).Build());
            table.Tr(attr => table.Field<Text>(f => f.AddressEn).ColSpan(3).ReadOnly(!isEdit).Build());
            table.Tr(attr =>
            {
                table.Field<Text>(f => f.Contact).ReadOnly(!isEdit).Build();
                table.Field<Text>(f => f.Phone).ReadOnly(!isEdit).Build();
            });
            table.Tr(attr => table.Field<TextArea>(f => f.Note).ColSpan(3).ReadOnly(!isEdit).Build());
        });
    }

    protected override void BuildButtons(RenderTreeBuilder builder)
    {
        if (!HasButton(FormButton.Edit))
            return;

        if (!isEdit)
        {
            builder.Button(FormButton.Edit, Callback(e => isEdit = true));
        }
        else
        {
            builder.Button(FormButton.Save, Callback(OnSaveInfo));
            builder.Button(FormButton.Cancel, Callback(e => isEdit = false));
        }
    }

    private void OnSaveInfo()
    {
        SubmitAsync(Platform.Company.SaveCompanyAsync, result =>
        {
            isEdit = false;
            StateChanged();
        });
    }
}