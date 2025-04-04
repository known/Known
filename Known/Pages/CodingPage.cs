
namespace Known.Pages;

/// <summary>
/// 代码生成开发插件页面组件类。
/// </summary>
[Route("/dev/coding")]
[DevPlugin("代码生成", "code", Sort = 98)]
public class CodingPage : BaseTabPage
{
    private ICodeService Service;
    private List<CodeModelInfo> Models = [];
    private CodeModelInfo Model = new();
    private string currentTab = "模型设置";

    [Inject] private ICodeGenerator Generator { get; set; }

    /// <inheritdoc />
    protected override async Task OnInitPageAsync()
    {
        if (!CurrentUser.IsSystemAdmin())
        {
            Navigation.GoErrorPage("403");
            return;
        }

        await base.OnInitPageAsync();
        Service = await CreateServiceAsync<ICodeService>();

        Tab.Class = "kui-coding";
        Tab.AddTab("模型设置", BuildModel);
        Tab.AddTab("建表脚本", BuildCode);
        Tab.AddTab("信息类", BuildCode);
        Tab.AddTab("实体类", BuildCode);
        Tab.AddTab("页面组件", BuildCode);
        Tab.AddTab("表单组件", BuildCode);
        Tab.AddTab("服务接口", BuildCode);
        Tab.AddTab("服务实现", BuildCode);

        Tab.OnChange = tab =>
        {
            currentTab = tab;
            StateChanged();
        };
    }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
            Models = await Service.GetModelsAsync();
    }

    private void BuildModel(RenderTreeBuilder builder)
    {
        builder.Component<KListPanel>()
               .Set(c => c.ListData, [.. Models.Select(m => new CodeInfo(m.Id, m.Name))])
               .Set(c => c.OnListClick, this.Callback<CodeInfo>(OnItemClickAsync))
               .Set(c => c.ChildContent, BuildModelForm)
               .Build();
    }

    private void BuildModelForm(RenderTreeBuilder builder)
    {
        builder.Component<CodeModelForm>()
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
        UI.Result(result);
    }

    private static string GetLanguage(string name)
    {
        return name switch
        {
            "建表脚本" => "sql",
            "表单组件" => "html",
            _ => "csharp"
        };
    }

    private string GenerateCode(string name)
    {
        var entity = Model.ToEntity();
        var page = Model.ToPage();
        var form = Model.ToForm();
        return name switch
        {
            "建表脚本" => Generator.GetScript(Config.DatabaseType, entity),
            "信息类" => Generator.GetModel(entity),
            "实体类" => Generator.GetEntity(entity),
            "页面组件" => Generator.GetPage(page, entity),
            "表单组件" => Generator.GetForm(form, entity),
            "服务接口" => Generator.GetIService(page, entity, true),
            "服务实现" => Generator.GetService(page, entity),
            _ => ""
        };
    }
}