namespace Known.Pages;

/// <summary>
/// 安装页面表单组件类。
/// </summary>
public class InstallForm : BaseForm<InstallInfo>
{
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
        Model = new FormModel<InstallInfo>(this);
        Model.Data = await Admin.GetInstallAsync();
        if (Model.Data.IsInstalled)
        {
            Navigation?.GoLoginPage();
            return;
        }

        if (Model.Data.IsDatabase)
            Step.AddStep("Database", BuildDatabase);
        Step.AddStep("SystemInfo", BuildSystem);
        Step.AddStep("AccountInfo", BuildAccount);
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
                    builder.Div("kui-app-name", $"{Config.App.Name} - {Language["Install"]}");
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
            builder.Div("kui-install-foot", () => builder.Component<KFooter>().Build());
        });
    }

    private void BuildDatabase(RenderTreeBuilder builder)
    {
        if (Model.Data == null || Model.Data.Databases == null || Model.Data.Databases.Count == 0)
            return;

        foreach (var database in Model.Data.Databases)
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

    private async Task OnTestAsync(DatabaseInfo info)
    {
        var result = await Admin.TestConnectionAsync(info);
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
            var result = await Admin.SaveInstallAsync(Model.Data);
            UI.Result(result, () =>
            {
                var info = result.DataAs<InstallInfo>();
                OnInstall?.Invoke(info);
                Navigation?.GoLoginPage();
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