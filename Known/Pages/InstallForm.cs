namespace Known.Pages;

public class InstallForm : BaseForm<InstallInfo>
{
    private ISystemService Service;
    private readonly StepModel Step = new();
    private readonly Dictionary<string, FormDatabase> formDBs = [];

    [Parameter] public RenderFragment TopMenu { get; set; }
    [Parameter] public Action<InstallInfo> OnInstall { get; set; }

    protected override async Task OnInitFormAsync()
    {
        await base.OnInitFormAsync();
        Service = await CreateServiceAsync<ISystemService>();
        Step.AddStep("Title.Database", BuildDatabase);
        Step.AddStep("Title.SystemInfo", BuildSystem);
        Step.AddStep("Title.AccountInfo", BuildAccount);
        Model = new FormModel<InstallInfo>(this);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            Model.Data = await Service.GetInstallAsync();
            await StateChangedAsync();
        }
    }

    protected override void BuildForm(RenderTreeBuilder builder)
    {
        builder.Div("kui-install", () =>
        {
            builder.Div("kui-install-head", () =>
            {
                builder.Div("", $"{Config.App.Name} - {Language["Install"]}");
                builder.Fragment(TopMenu);
            });
            builder.Div("kui-install-body", () =>
            {
                builder.Div("kui-install-form", () =>
                {
                    builder.Cascading(this, b =>
                    {
                        b.Component<StepForm>()
                         .Set(c => c.Model, Step)
                         .Set(c => c.StepCount, Step.Items.Count)
                         .Set(c => c.IsStepSave, true)
                         .Set(c => c.OnSave, SaveAsync)
                         .Build();
                    });
                });
            });
            builder.Div("kui-install-foot", () => builder.Component<PageFooter>().Build());
        });
    }

    private void BuildDatabase(RenderTreeBuilder builder)
    {
        if (Model.Data == null || Model.Data.Databases == null || Model.Data.Databases.Count == 0)
            return;

        foreach (var database in Model.Data.Databases)
        {
            builder.Component<FormDatabase>()
                   .Set(c => c.Service, Service)
                   .Set(c => c.Data, database)
                   .Build(value => formDBs[database.Name] = value);
        }
    }

    private void BuildSystem(RenderTreeBuilder builder)
    {
        builder.Component<FormSystem>().Set(c => c.Model, Model).Build();
    }

    private void BuildAccount(RenderTreeBuilder builder)
    {
        builder.Component<FormAccount>().Set(c => c.Model, Model).Build();
    }

    private async Task<bool> SaveAsync(bool isComplete = false)
    {
        if (Step.Current == 0)
        {
            if (!ValidateDatabase())
                return false;
        }

        if (!Model.Validate())
            return false;

        if (isComplete)
        {
            var result = await Service.SaveInstallAsync(Model.Data);
            UI.Result(result, () =>
            {
                var info = result.DataAs<InstallInfo>();
                OnInstall?.Invoke(info);
                return Task.CompletedTask;
            });
        }
        return true;
    }

    private bool ValidateDatabase()
    {
        foreach (var item in formDBs)
        {
            if (!item.Value.Validate())
                return false;
        }

        return true;
    }
}

class FormDatabase : BaseForm<DatabaseInfo>
{
    [Parameter] public ISystemService Service { get; set; }
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
        builder.Div("kui-right", () => builder.Button(Language["Designer.Test"], this.Callback<MouseEventArgs>(OnTest), "primary"));
    }

    private async void OnTest(MouseEventArgs args)
    {
        var result = await Service.TestConnectionAsync(Model.Data);
        await UI.ResultAsync(result);
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