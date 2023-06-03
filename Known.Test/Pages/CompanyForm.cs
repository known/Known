namespace Known.Test.Pages;

class CompanyForm : BaseForm<CompanyInfo>
{
    private bool isEdit = false;

    public CompanyForm()
    {
        IsInline = true;
        Style = "company";
    }

    protected override async Task InitPageAsync()
    {
        Model = await Platform.Company.GetCompanyAsync<CompanyInfo>();
    }

    protected override void BuildFields(FieldBuilder<CompanyInfo> builder)
    {
        builder.Hidden(f => f.Code);
        builder.Field<Text>(f => f.Name).ReadOnly(!isEdit).Build();
        builder.Field<Text>(f => f.NameEn).ReadOnly(!isEdit).Build();
        builder.Field<Text>(f => f.Address).ColSpan(3).ReadOnly(!isEdit).Build();
        builder.Field<Text>(f => f.AddressEn).ColSpan(3).ReadOnly(!isEdit).Build();
        builder.Field<Text>(f => f.Contact).ReadOnly(!isEdit).Build();
        builder.Field<Text>(f => f.Phone).ReadOnly(!isEdit).Build();
        builder.Field<TextArea>(f => f.Note).ColSpan(3).ReadOnly(!isEdit).Build();
        BuildButton(builder.Builder);
    }

    private void BuildButton(RenderTreeBuilder builder)
    {
        if (!HasButton(FormButton.Edit))
            return;

        builder.Div("form-button", attr =>
        {
            if (!isEdit)
            {
                builder.Button(FormButton.Edit, Callback(e => isEdit = true));
            }
            else
            {
                builder.Button(FormButton.Save, Callback(OnSaveInfo));
                builder.Button(FormButton.Cancel, Callback(e => isEdit = false));
            }
        });
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