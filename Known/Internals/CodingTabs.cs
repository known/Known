namespace Known.Internals;

/// <summary>
/// 生成代码标签页组件类。
/// </summary>
public class CodingTabs : BaseComponent
{
    private const string TabScript = "建表脚本";
    private const string TabInfo = "信息类";
    private const string TabEntity = "实体类";
    private const string TabPage = "页面组件";
    private const string TabForm = "表单组件";
    private const string TabServiceI = "服务接口";
    private const string TabService = "服务实现";

    private readonly TabModel Tab = new();
    private IAutoService Service;
    private string currentTab = "";

    [Inject] private ICodeGenerator Generator { get; set; }

    /// <summary>
    /// 取得或设置标题。
    /// </summary>
    [Parameter] public string Title { get; set; }

    /// <summary>
    /// 取得或设置代码生成模型信息。
    /// </summary>
    [Parameter] public CodeModelInfo Model { get; set; }

    /// <summary>
    /// 取得或设置模型配置标签字典。
    /// </summary>
    [Parameter] public Dictionary<string, RenderFragment> ModelTabs { get; set; }

    /// <inheritdoc />
    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        Service = await CreateServiceAsync<IAutoService>();

        currentTab = ModelTabs.Keys.First();
        Tab.Class = Class;
        foreach (var tab in ModelTabs)
        {
            Tab.AddTab(tab.Key, tab.Value);
        }
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
        if (!string.IsNullOrWhiteSpace(Title))
            Tab.Left = b => b.FormTitle(Title);
        Tab.Right = BuildTabRight;
    }

    /// <inheritdoc />
    protected override void BuildRender(RenderTreeBuilder builder)
    {
        if (!string.IsNullOrWhiteSpace(Title))
            builder.Div("kui-card", () => builder.Tabs(Tab));
        else
            builder.Tabs(Tab);
    }

    private void BuildTabRight(RenderTreeBuilder builder)
    {
        if (ModelTabs.ContainsKey(currentTab))
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

    private void BuildCode(RenderTreeBuilder builder)
    {
        var lang = GetLanguage(currentTab);
        var code = GenerateCode(currentTab);
        builder.Component<KCodeView>().Set(c => c.Lang, lang).Set(c => c.Code, code).Build();
    }

    private async Task OnExecute(MouseEventArgs args)
    {
        var code = GenerateCode(currentTab);
        var info = new AutoInfo<string> { PageId = Model.EntityName, Data = code };
        var result = await Service.CreateTableAsync(info);
        UI.Result(result);
    }

    private async Task OnSaveCode(MouseEventArgs args)
    {
        var path = GetCodePath(currentTab);
        var code = GenerateCode(currentTab);
        var info = new AutoInfo<string> { PageId = path, Data = code };
        var result = await Service.SaveCodeAsync(info);
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