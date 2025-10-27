namespace Known.Pages;

/// <summary>
/// 安装页面表单组件类。
/// </summary>
[Route("/install")]
[Layout(typeof(EmptyLayout))]
public class InstallPage : BaseForm<InstallInfo>
{
    private IInstallService Service;
    private readonly StepModel Step = new();
    private readonly Dictionary<string, FormDatabase> formDBs = [];

    /// <inheritdoc />
    protected override async Task OnInitFormAsync()
    {
        await base.OnInitFormAsync();
        Service = await CreateServiceAsync<IInstallService>();

        Model = new FormModel<InstallInfo>(this);
        Model.Data = await Service.GetInstallAsync();
        if (Model.Data.IsInstalled)
        {
            Navigation.GoLoginPage();
        }
        else
        {
            if (Model.Data.IsDatabase)
                Step.AddStep(Language.Database, BuildDatabase);
            Step.AddStep(Language.SystemInfo, BuildSystem);
            Step.AddStep(Language.AccountInfo, BuildAccount);
        }
    }

    /// <inheritdoc />
    protected override void BuildForm(RenderTreeBuilder builder)
    {
        if (Model.Data.IsInstalled)
            return;

        builder.Div("kui-install", () =>
        {
            builder.Div("kui-install-head", () =>
            {
                builder.Div("kui-flex", () =>
                {
                    builder.Div().Class("kui-logo").Close();
                    builder.Div("kui-app-name", $"{Language[Config.App.Name]} - {Language[Language.Install]}");
                });
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
            builder.Div("kui-install-foot", () => builder.Component<KFooter>().Build());
        });
    }

    private void BuildDatabase(RenderTreeBuilder builder)
    {
        if (Model.Data == null || Model.Data.Connections == null || Model.Data.Connections.Count == 0)
            return;

        foreach (var database in Model.Data.Connections)
        {
            builder.Component<FormDatabase>()
                   .Set(c => c.Info, database)
                   .Set(c => c.OnTest, OnTestAsync)
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

    private async Task OnTestAsync(ConnectionInfo info)
    {
        var result = await Service.TestConnectionAsync(info);
        UI.Result(result);
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
                Navigation.GoLoginPage();
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