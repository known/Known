namespace Known.Pages;

/// <summary>
/// 代码生成开发插件页面组件类。
/// </summary>
[Route("/dev/coding")]
[DevPlugin("代码生成", "code", Sort = 98)]
public class CodingPage : BaseTabPage
{
    private const string TabModel = "模型设置";
    private const string TabScript = "建表脚本";
    private const string TabInfo = "信息类";
    private const string TabEntity = "实体类";
    private const string TabPage = "页面组件";
    private const string TabForm = "表单组件";
    private const string TabServiceI = "服务接口";
    private const string TabService = "服务实现";

    private IAutoService AutoService;
    private ICodeService Service;
    private List<CodeModelInfo> Models = [];
    private CodeModelInfo Model = new();
    private KListPanel listPanel;
    private string currentTab = "模型设置";

    [Inject] private ICodeGenerator Generator { get; set; }

    private List<CodeInfo> ListData => [.. Models?.Select(m => new CodeInfo(m.Id, m.Name))];

    /// <inheritdoc />
    protected override async Task OnInitPageAsync()
    {
        if (!CurrentUser.IsSystemAdmin())
        {
            Navigation.GoErrorPage("403");
            return;
        }

        await base.OnInitPageAsync();
        AutoService = await CreateServiceAsync<IAutoService>();
        Service = await CreateServiceAsync<ICodeService>();

        Tab.Class = "kui-coding";
        Tab.AddTab(TabModel, BuildModel);
        Tab.AddTab(TabScript, BuildCode);
        Tab.AddTab(TabInfo, BuildCode);
        Tab.AddTab(TabEntity, BuildCode);
        Tab.AddTab(TabPage, BuildCode);
        Tab.AddTab(TabForm, BuildCode);
        Tab.AddTab(TabServiceI, BuildCode);
        Tab.AddTab(TabService, BuildCode);

        Tab.OnChange = tab =>
        {
            currentTab = tab;
            StateChanged();
        };
        Tab.Right = BuildTabRight;
    }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            Models = await Service.GetModelsAsync();
            Model = Models.FirstOrDefault();
        }
    }

    private void BuildTabRight(RenderTreeBuilder builder)
    {
        if (currentTab == TabModel)
            return;

        if (currentTab == TabScript)
        {
            builder.Button("执行", this.Callback<MouseEventArgs>(OnExecute));
        }
        else
        {
            var path = GetCodePath(currentTab);
            var file = GetCodeFile(currentTab);
            builder.Tooltip(path, b => b.Tag(file));
            builder.Button("保存", this.Callback<MouseEventArgs>(OnSaveCode));
        }
    }

    private void BuildModel(RenderTreeBuilder builder)
    {
        builder.Component<KListPanel>()
               .Set(c => c.ListData, ListData)
               .Set(c => c.ListTemplate, this.BuildTree<CodeInfo>((b, c) => b.Text(c.Name)))
               .Set(c => c.OnListClick, this.Callback<CodeInfo>(OnItemClickAsync))
               .Set(c => c.ChildContent, BuildModelForm)
               .Build(value => listPanel = value);
    }

    private void BuildModelForm(RenderTreeBuilder builder)
    {
        builder.Component<CodeModelForm>()
               .Set(c => c.Default, Models.FirstOrDefault())
               .Set(c => c.Model, Model)
               .Set(c => c.OnModelSave, this.Callback<CodeModelInfo>(OnSaveModelAsync))
               .Build();
    }

    private void BuildCode(RenderTreeBuilder builder)
    {
        var lang = GetLanguage(currentTab);
        var code =  GenerateCode(currentTab);
        builder.Component<KCodeView>().Set(c => c.Lang, lang).Set(c => c.Code, code).Build();
    }

    private Task OnItemClickAsync(CodeInfo info)
    {
        Model = Models.FirstOrDefault(m => m.Id == info.Code);
        return StateChangedAsync();
    }

    private async Task OnSaveModelAsync(CodeModelInfo info)
    {
        var result = await Service.SaveModelAsync(info);
        UI.Result(result, async () =>
        {
            Models = await Service.GetModelsAsync();
            Model = Models.FirstOrDefault(m => m.Id == info.Id);
            listPanel?.SetListData(ListData);
        });
    }

    private async Task OnExecute(MouseEventArgs args)
    {
        var code = GenerateCode(currentTab);
        var info = new AutoInfo<string> { PageId = Model.EntityName, Data = code };
        var result = await AutoService.CreateTableAsync(info);
        UI.Result(result);
    }

    private async Task OnSaveCode(MouseEventArgs args)
    {
        var path = GetCodePath(currentTab);
        var code = GenerateCode(currentTab);
        var info = new AutoInfo<string> { PageId = path, Data = code };
        var result = await AutoService.SaveCodeAsync(info);
        UI.Result(result);
    }

    private static string GetLanguage(string name)
    {
        return name switch
        {
            TabForm => "html",
            _ => "csharp"
        };
    }

    private string GetCodePath(string name)
    {
        return name switch
        {
            TabInfo => Model.ModelPath,
            TabEntity => Model.EntityPath,
            TabPage => Model.PagePath,
            TabForm => Model.FormPath,
            TabServiceI => Model.ServiceIPath,
            TabService => Model.ServicePath,
            _ => ""
        };
    }

    private string GetCodeFile(string name)
    {
        return name switch
        {
            TabInfo => $"{Model.ModelName}.cs",
            TabEntity => $"{Model.EntityName}.cs",
            TabPage => $"{Model.PageName}.cs",
            TabForm => $"{Model.FormName}.razor",
            TabServiceI => $"{Model.ServiceName}.cs",
            TabService => $"{Model.ServiceName}.cs",
            _ => ""
        };
    }

    private string GenerateCode(string name)
    {
        var entity = Model.ToEntity();
        var page = Model.ToPage();
        var form = Model.ToForm();
        Generator.Model = Model;
        return name switch
        {
            TabScript => Generator.GetScript(Config.DatabaseType, entity),
            TabInfo => Generator.GetModel(entity),
            TabEntity => Generator.GetEntity(entity),
            TabPage => Generator.GetPage(page, entity),
            TabForm => Generator.GetForm(form, entity),
            TabServiceI => Generator.GetIService(page, entity, true),
            TabService => Generator.GetService(page, entity),
            _ => ""
        };
    }
}