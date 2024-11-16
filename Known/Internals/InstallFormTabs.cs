namespace Known.Internals;

class FormDatabase : BaseForm<DatabaseInfo>
{
    [Parameter] public DatabaseInfo Data { get; set; }

    protected override async Task OnInitFormAsync()
    {
        await base.OnInitFormAsync();
        Model = new FormModel<DatabaseInfo>(this) { SmallLabel = true, Data = Data };
        Model.AddRow().AddColumn(c => c.Name, c => c.ReadOnly = true);
        Model.AddRow().AddColumn(c => c.Type, c => c.ReadOnly = true);
        Model.AddRow().AddColumn(c => c.ConnectionString, c =>
        {
            c.Required = true;
            c.Type = FieldType.TextArea;
        });
    }

    public bool Validate() => Model.Validate();

    protected override void BuildForm(RenderTreeBuilder builder)
    {
        base.BuildForm(builder);
        builder.Div("kui-right", () => builder.Button(Language["Test"], this.Callback<MouseEventArgs>(OnTestAsync)));
    }

    private async Task OnTestAsync(MouseEventArgs args)
    {
        var result = await System.TestConnectionAsync(Model.Data);
        UI.Result(result);
    }
}

class FormSystem : BaseForm<InstallInfo>
{
    protected override async Task OnInitFormAsync()
    {
        await base.OnInitFormAsync();
        Model.SmallLabel = true;
        Model.Rows.Clear();
        Model.AddRow().AddColumn(c => c.CompNo, c => c.Required = true);
        Model.AddRow().AddColumn(c => c.CompName, c => c.Required = true);
        Model.AddRow().AddColumn(c => c.AppName, c => c.Required = true);
    }
}

class FormAccount : BaseForm<InstallInfo>
{
    protected override async Task OnInitFormAsync()
    {
        await base.OnInitFormAsync();
        Model.SmallLabel = true;
        Model.Rows.Clear();
        Model.AddRow().AddColumn(c => c.AdminName, c => c.ReadOnly = true);
        Model.AddRow().AddColumn(c => c.AdminPassword, c =>
        {
            c.Required = true;
            c.Type = FieldType.Password;
        });
        Model.AddRow().AddColumn(c => c.Password1, c =>
        {
            c.Required = true;
            c.Type = FieldType.Password;
        });
    }
}