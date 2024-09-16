

namespace Known.Pages;

/// <summary>
/// 代码生成表单组件类。
/// </summary>
public class CodeGenerateForm : BasePage
{
    private List<CodeInfo> models = [];
    private CodeInfo model;

    /// <summary>
    /// 异步初始化组件。
    /// </summary>
    /// <returns></returns>
    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
    }

    /// <summary>
    /// 页面呈现后，调用后台数据。
    /// </summary>
    /// <param name="firstRender">是否首次呈现。</param>
    /// <returns></returns>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
            await LoadModelsAsync();
    }

    /// <summary>
    /// 构建页面内容。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    protected override void BuildPage(RenderTreeBuilder builder)
    {
        builder.Div("kui-row-28", () =>
        {
            BuildModels(builder);
            BuildTabForm(builder);
        });
    }

    private void BuildModels(RenderTreeBuilder builder)
    {
        builder.Component<KListBox>()
               .Set(c => c.ShowSearch, true)
               .Set(c => c.DataSource, models)
               .Set(c => c.ItemTemplate, ItemTemplate)
               .Set(c => c.OnItemClick, OnItemClick)
               .Build();
    }

    private void BuildTabForm(RenderTreeBuilder builder) => builder.Component<CodeGenerateTabs>().Build();

    private RenderFragment ItemTemplate(CodeInfo info) => b => b.Text($"{info.Name} ({info.Code})");

    private Task OnItemClick(CodeInfo info)
    {
        //category = info;
        return Task.CompletedTask;
    }

    private async Task LoadModelsAsync()
    {
        //models = await Service.GetCategoriesAsync();
        for (int i = 0; i < 60; i++)
        {
            models.Add(new CodeInfo($"Test{i}", $"测试模型{i}"));
        }
        model ??= models?.FirstOrDefault();
        await OnItemClick(model);
        await StateChangedAsync();
    }
}

class CodeGenerateTabs : BaseComponent
{
    private TabModel Tab { get; } = new();

    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        Tab.AddTab("Designer.Models", BuildForm);
        Tab.AddTab("Designer.EntityCode", BuildEntity);
        Tab.AddTab("Designer.PageCode", BuildPage);
        Tab.AddTab("Designer.ServiceCode", BuildService);
        Tab.AddTab("Designer.RepositoryCode", BuildRepository);
    }

    protected override void BuildRender(RenderTreeBuilder builder) => UI.BuildTabs(builder, Tab);

    private void BuildForm(RenderTreeBuilder builder)
    {
    }

    private void BuildEntity(RenderTreeBuilder builder)
    {
    }

    private void BuildPage(RenderTreeBuilder builder)
    {
    }

    private void BuildService(RenderTreeBuilder builder)
    {
    }

    private void BuildRepository(RenderTreeBuilder builder)
    {
    }
}