namespace Known.Demo.Pages.BaseData;

class CompanyForm : WebForm<CompanyInfo>
{
    private bool isEdit = false;

    public CompanyForm()
    {
        TabItems = new List<KMenuItem>
        {
            new KMenuItem("BaseInfo", "基本信息"),
            new KMenuItem("Qualifications", "企业资质")
        };
    }

    protected override async Task InitFormAsync()
    {
        Model = await Platform.GetCompanyAsync<CompanyInfo>();
    }

    protected override void BuildFields(FieldBuilder<CompanyInfo> builder)
    {
        builder.Hidden(f => f.Code);
        builder.Table(table =>
        {
            table.ColGroup(15, 35, 15, 35);
            table.Tr(attr =>
            {
                table.Field<KText>(f => f.Name).ReadOnly(!isEdit).Build();
                table.Field<KText>(f => f.NameEn).ReadOnly(!isEdit).Build();
            });
            table.Tr(attr => table.Field<KText>(f => f.Address).ColSpan(3).ReadOnly(!isEdit).Build());
            table.Tr(attr => table.Field<KText>(f => f.AddressEn).ColSpan(3).ReadOnly(!isEdit).Build());
            table.Tr(attr =>
            {
                table.Field<KText>(f => f.Contact).ReadOnly(!isEdit).Build();
                table.Field<KText>(f => f.Phone).ReadOnly(!isEdit).Build();
            });
            table.Tr(attr => table.Field<KTextArea>(f => f.Note).ColSpan(3).ReadOnly(!isEdit).Build());
        });
        BuildButton(builder.Builder);
    }

    protected override void BuildTabBody(RenderTreeBuilder builder, KMenuItem item)
    {
        builder.Span(item.Name);
    }

    protected override void BuildButtons(RenderTreeBuilder builder) { }

    private void BuildButton(RenderTreeBuilder builder)
    {
        if (!HasButton(FormButton.Edit))
            return;

        builder.Div("company-buttons", attr =>
        {
            if (!isEdit)
            {
                builder.Button(FormButton.Edit, Callback(e => isEdit = true));
            }
            else
            {
                builder.Button(FormButton.Save, Callback(OnSaveInfoAsync));
                builder.Button(FormButton.Cancel, Callback(e => isEdit = false));
            }
        });
    }

    private async Task OnSaveInfoAsync()
    {
        await SubmitAsync(Platform.SaveCompanyAsync, result =>
        {
            isEdit = false;
            StateChanged();
        });
    }
}