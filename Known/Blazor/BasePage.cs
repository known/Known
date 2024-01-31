using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Blazor;

public class BasePage : BaseComponent
{
    private AutoTablePage page;

    [Parameter] public string PageId { get; set; }

    public string PageName => Language.GetString(Context.Module);

    public virtual Task RefreshAsync() => Task.CompletedTask;

    public override void StateChanged()
    {
        base.StateChanged();
        page?.StateChanged();
    }

    protected override async Task OnInitializedAsync()
    {
        try
        {
            await base.OnInitializedAsync();
            await OnInitPageAsync();
            await AddVisitLogAsync();
        }
        catch (Exception ex)
        {
            await Error?.HandleAsync(ex);
        }
    }

    protected virtual async Task OnInitPageAsync()
    {
        if (!string.IsNullOrWhiteSpace(PageId))
            Context.Module = await Platform.Module.GetModuleAsync(PageId);
    }

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
        if (!string.IsNullOrWhiteSpace(PageId))
            Context.Module = await Platform.Module.GetModuleAsync(PageId);
    }

    protected override async void BuildRenderTree(RenderTreeBuilder builder)
    {
        try
        {
            BuildPage(builder);
        }
        catch (Exception ex)
        {
            await Error?.HandleAsync(ex);
        }
    }

    protected virtual void BuildPage(RenderTreeBuilder builder)
    {
        if (Context.Module != null && Context.Module.Target == "Page")
            builder.Component<AutoTablePage>().Set(c => c.PageId, PageId).Build(value => page = value);
        else
            UI.BuildResult(builder, "404", $"{Language["Tip.Page404"]}PageId={PageId}");
    }
}

public class BasePage<TItem> : BasePage where TItem : class, new()
{
    protected PageModel Page { get; } = new();
    internal List<string> Tools { get; set; }
    internal List<string> Actions { get; set; }
    internal List<PageColumnInfo> Columns { get; set; }

    internal virtual void ViewForm(FormType type, TItem row) { }

    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();
        InitMenu();
    }

    protected override void BuildPage(RenderTreeBuilder builder)
    {
        builder.Component<WebPage>().Set(c => c.Model, Page).Build();
    }

    protected void OnToolClick(ActionInfo info) => OnAction(info, null);
    protected void OnActionClick(ActionInfo info, TItem item) => OnAction(info, [item]);

    private async void OnAction(ActionInfo info, object[] parameters)
    {
        var type = GetType();
        var method = type.GetMethod(info.Id);
        if (method == null)
        {
            var message = Language["Tip.NoMethod"].Replace("{method}", $"{info.Name}[{type.Name}.{info.Id}]");
            UI.Error(message);
            return;
        }

        try
        {
            method.Invoke(this, parameters);
        }
        catch (Exception ex)
        {
            await Error?.HandleAsync(ex);
        }
    }

    internal void InitMenu()
    {
        if (Context == null || Context.UserMenus == null)
            return;

        var menu = Context.UserMenus.FirstOrDefault(m => m.Id == PageId);
        if (menu == null)
            return;

        Id = menu.Id;
        Name = menu.Name;
        Tools = menu.Buttons;
        Actions = menu.Actions;
        Columns = menu.Columns;
    }
}

public class BaseTablePage<TItem> : BasePage<TItem> where TItem : class, new()
{
    protected TableModel<TItem> Table { get; private set; }

    public override Task RefreshAsync() => Table.RefreshAsync();
    internal override void ViewForm(FormType type, TItem row) => Table.ViewForm(type, row);

    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();
        Table = new TableModel<TItem>(this) { OnAction = OnActionClick };
        Table.Toolbar.OnItemClick = OnToolClick;
    }

    protected override void BuildPage(RenderTreeBuilder builder) => builder.BuildTablePage(Table);

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
        Table.Criteria.ExportMode = mode;
        Table.Criteria.ExportColumns = GetExportColumns();
        var result = await Table.OnQuery?.Invoke(Table.Criteria);
        var bytes = result.ExportData;
        if (bytes == null || bytes.Length == 0)
            return;

        var stream = new MemoryStream(bytes);
        await JS.DownloadFileAsync($"{name}.xlsx", stream);
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

    protected override void BuildPage(RenderTreeBuilder builder) => UI.BuildTabs(builder, Tab);
}