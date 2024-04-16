using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Blazor;

public class BasePage : BaseComponent
{
    private AutoTablePage page;
    internal MenuInfo Menu { get; set; }

    [Parameter] public string PageId { get; set; }

    public string PageName => Language.GetString(Context.Module);

    public virtual Task RefreshAsync() => Task.CompletedTask;

    public override void StateChanged()
    {
        base.StateChanged();
        page?.StateChanged();
    }

    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        await OnInitPageAsync();
        await AddVisitLogAsync();
    }

    protected virtual async Task OnInitPageAsync()
    {
        if (!string.IsNullOrWhiteSpace(PageId))
            Context.Module = await Platform.Module.GetModuleAsync(PageId);
        InitMenu();
    }

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
        if (!string.IsNullOrWhiteSpace(PageId))
            Context.Module = await Platform.Module.GetModuleAsync(PageId);
    }

    protected override void BuildRender(RenderTreeBuilder builder)
    {
        builder.Component<KAuthPanel>().Set(c => c.ChildContent, BuildPage).Build();
    }

    protected virtual void BuildPage(RenderTreeBuilder builder)
    {
        if (Context.Module == null)
        {
            UI.Build404Page(builder, PageId);
            return;
        }

        var type = Utils.ConvertTo<ModuleType>(Context.Module.Target);
        if (type == ModuleType.Page)
            builder.Component<AutoTablePage>().Set(c => c.PageId, PageId).Build(value => page = value);
        else if (type == ModuleType.Custom)
            UI.Build404Page(builder, PageId);
        else if (type == ModuleType.IFrame)
            builder.IFrame(Context.Module.Url);
    }

    protected void OnToolClick(ActionInfo info) => OnAction(info, null);
    protected void OnActionClick<TModel>(ActionInfo info, TModel item) => OnAction(info, [item]);

    internal void InitMenu()
    {
        if (Context == null || Context.UserMenus == null)
            return;

        Menu = Context.UserMenus.FirstOrDefault(m => m.Id == PageId);
        if (Menu == null)
            return;

        Id = Menu.Id;
        Name = Menu.Name;
    }
}

public class BasePage<TItem> : BasePage where TItem : class, new()
{
    protected PageModel Page { get; } = new();

    internal virtual void ViewForm(FormViewType type, TItem row) { }

    protected override void BuildPage(RenderTreeBuilder builder)
    {
        builder.Component<WebPage>().Set(c => c.Model, Page).Build();
    }

    protected void OnActionClick(ActionInfo info, TItem item) => OnAction(info, [item]);
}

public class BaseTablePage<TItem> : BasePage<TItem> where TItem : class, new()
{
    protected TableModel<TItem> Table { get; set; }

    public IEnumerable<TItem> SelectedRows => Table.SelectedRows;

    public override Task RefreshAsync() => Table.RefreshAsync();
    internal override void ViewForm(FormViewType type, TItem row) => Table.ViewForm(type, row);

    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();
        Table = new TableModel<TItem>(this);
        Table.OnAction = OnActionClick;
        Table.Toolbar.OnItemClick = OnToolClick;
    }

    protected override void BuildPage(RenderTreeBuilder builder) => builder.BuildTable(Table);

    protected async void ShowImportForm(string param = null)
    {
        var type = typeof(TItem);
        var id = $"{type.Name}Import";
        if (!string.IsNullOrWhiteSpace(param))
            id += $"_{param}";
        if (Table.IsDictionary)
            id += $"_{Context.Current.Id}";
        var info = await Platform.File.GetImportAsync(id);
        info.Name = PageName;
        info.BizName = ImportTitle;
        ImportForm(info);
    }

    private void ImportForm(ImportFormInfo info)
    {
        var model = new DialogModel { Title = ImportTitle };
        model.Content = builder =>
        {
            builder.Component<Importer>()
                   .Set(c => c.Model, info)
                   .Set(c => c.OnSuccess, async () =>
                   {
                       await model.CloseAsync();
                       await RefreshAsync();
                   })
                   .Build();
        };
        UI.ShowDialog(model);
    }

    private string ImportTitle => Language["Title.Import"].Replace("{name}", PageName);

    protected Task ExportDataAsync(ExportMode mode) => ExportDataAsync(PageName, mode);

    protected async Task ExportDataAsync(string name, ExportMode mode)
    {
        try
        {
            await Admin.ShowSpinAsync();
            Table.Criteria.ExportMode = mode;
            Table.Criteria.ExportColumns = GetExportColumns();
            var result = await Table.OnQuery?.Invoke(Table.Criteria);
            var bytes = result.ExportData;
            if (bytes == null || bytes.Length == 0)
                return;

            var stream = new MemoryStream(bytes);
            await JS.DownloadFileAsync($"{name}.xlsx", stream);
            Admin.HideSpin();
        }
        catch (Exception ex)
        {
            await Error.HandleAsync(ex);
            Admin.HideSpin();
        }
    }

    private Dictionary<string, string> GetExportColumns()
    {
        var columns = new Dictionary<string, string>();
        if (Table.Columns == null || Table.Columns.Count == 0)
            return columns;

        foreach (var item in Table.Columns)
        {
            columns.Add(item.Id, item.Name);
        }
        return columns;
    }
}

public class BaseTabPage : BasePage
{
    protected TabModel Tab { get; } = new();

    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();
        Tab.Left = b => b.Component<KTitle>().Set(c => c.Text, Name).Build();
    }

    protected override void BuildPage(RenderTreeBuilder builder)
    {
        builder.Div("kui-card", () => UI.BuildTabs(builder, Tab));
    }
}

public class BaseStepPage : BasePage
{
    protected StepModel Step { get; } = new();

    protected override void BuildPage(RenderTreeBuilder builder)
    {
        builder.Div("kui-card", () => UI.BuildSteps(builder, Step));
    }
}