namespace Known.Pages;

/// <summary>
/// 代码生成开发插件页面组件类。
/// </summary>
[Route("/dev/coding")]
[DevPlugin("代码生成", "code", Sort = 98)]
public class CodingPage : BasePage
{
    private const string TabModel = "模型设置";

    private ICodeService Service;
    private List<CodeModelInfo> Models = [];
    private CodeModelInfo Model = new();
    private KListPanel listPanel;

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
        Service = await CreateServiceAsync<ICodeService>();
    }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            Models = await Service.GetModelsAsync();
            Model = Models.FirstOrDefault() ?? new CodeModelInfo();
            await StateChangedAsync();
        }
    }

    /// <inheritdoc />
    protected override void BuildPage(RenderTreeBuilder builder)
    {
        var tabs = new Dictionary<string, RenderFragment>
        {
            { TabModel, BuildModel }
        };
        builder.Component<CodingTabs>()
               .Set(c => c.Class, "kui-coding")
               .Set(c => c.Title, PageName)
               .Set(c => c.Model, Model)
               .Set(c => c.ModelTabs, tabs)
               .Set(c => c.OnTabChange, tab => Model.TransModels())
               .Build();
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
}