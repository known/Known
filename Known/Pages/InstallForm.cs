namespace Known.Pages;

/// <summary>
/// 安装页面表单组件类。
/// </summary>
public class InstallForm : BaseForm<InstallInfo>
{
    private ISystemService Service;
    private readonly StepModel Step = new();
    private readonly Dictionary<string, FormDatabase> formDBs = [];

    /// <summary>
    /// 取得或设置页面组件顶部菜单模板。
    /// </summary>
    [Parameter] public RenderFragment TopMenu { get; set; }

    /// <summary>
    /// 取得或设置安装成功后回调方法。
    /// </summary>
    [Parameter] public Action<InstallInfo> OnInstall { get; set; }

    /// <summary>
    /// 异步初始化安装表单。
    /// </summary>
    /// <returns></returns>
    protected override async Task OnInitFormAsync()
    {
        await base.OnInitFormAsync();

        if (Context.System != null)
        {
            App?.NavigateTo("/login");
            return;
        }

        Service = await CreateServiceAsync<ISystemService>();
        Step.AddStep("Title.Database", BuildDatabase);
        Step.AddStep("Title.SystemInfo", BuildSystem);
        Step.AddStep("Title.AccountInfo", BuildAccount);
        Model = new FormModel<InstallInfo>(this);
    }

    /// <summary>
    /// 安装表单呈现后，调用后端数据。
    /// </summary>
    /// <param name="firstRender">是否首次呈现。</param>
    /// <returns></returns>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            Model.Data = await Service.GetInstallAsync();
            await StateChangedAsync();
        }
    }

    /// <summary>
    /// 构建安装表单内容。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    protected override void BuildForm(RenderTreeBuilder builder)
    {
        builder.Div("kui-install", () =>
        {
            builder.Div("kui-install-head", () =>
            {
                builder.Div("kui-flex", () =>
                {
                    builder.Div().Class("kui-logo").Style("position:initial;height:55px;").Close();
                    builder.Div("app-name", $"{Config.App.Name} - {Language["Install"]}");
                });
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
        builder.Div("kui-right", () => builder.Button(Language["Designer.Test"], this.Callback<MouseEventArgs>(OnTest)));
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