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
               .Set(c => c.ListTemplate, this.BuildTree<CodeInfo>(BuildListItem))
               .Set(c => c.OnListClick, this.Callback<CodeInfo>(OnItemClickAsync))
               .Set(c => c.ChildContent, BuildModelForm)
               .Build(value => listPanel = value);
    }

    private void BuildListItem(RenderTreeBuilder builder, CodeInfo item)
    {
        builder.Div().Class("kui-flex-space").Child(() =>
        {
            builder.Div("", item.Name);
            if (item.IsActive)
            {
                builder.Dropdown(new DropdownModel
                {
                    Icon = "menu",
                    Items = [new() { Icon = "delete", Name = Language.Delete, OnClick = this.Callback<MouseEventArgs>(e=>OnDelete(item)) }]
                });
            }
        });
    }

    private void BuildModelForm(RenderTreeBuilder builder)
    {
        builder.Component<CodeModelForm>()
               .Set(c => c.Service, Service)
               .Set(c => c.Default, Models.FirstOrDefault())
               .Set(c => c.Model, Model)
               .Set(c => c.OnSaved, OnModelSaved)
               .Build();
    }

    private Task OnItemClickAsync(CodeInfo item)
    {
        Model = Models.FirstOrDefault(m => m.Id == item.Code);
        return StateChangedAsync();
    }

    private void OnModelSaved(CodeModelInfo info)
    {
        if (!Models.Exists(m => m.Id == info.Id))
            Models.Add(info);
        Model = Models.FirstOrDefault(m => m.Id == info.Id) ?? new CodeModelInfo();
        listPanel?.SetListBox(ListData, info.Id);
    }

    private void OnDelete(CodeInfo item)
    {
        UI.Confirm($"确定要删除【{item?.Name}】？", async () =>
        {
            var result = await Service.DeleteModelsAsync([item]);
            UI.Result(result, () =>
            {
                Models.RemoveAll(m => m.Id == item.Code);
                listPanel?.SetListBox(ListData, Model.Id);
                return Task.CompletedTask;
            });
        });
    }
}