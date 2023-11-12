using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Razor;

public class BasePage : BaseComponent
{
    [Parameter] public string PageId { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await OnInitPageAsync();
        await AddVisitLogAsync();
        await base.OnInitializedAsync();
    }

    protected virtual Task OnInitPageAsync() => Task.CompletedTask;

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        UI.BuildResult(builder, "404", $"页面不存在！PageId={PageId}");
    }
}

public class BasePage<TItem> : BasePage where TItem : class, new()
{
    private List<ActionInfo> Tools { get; set; }
    private List<ActionInfo> Actions { get; set; }
    private List<ColumnInfo> Columns { get; set; }

    protected TableModel<TItem> Table { get; private set; }
    protected TreeModel Tree { get; set; }

    protected override Task OnInitPageAsync()
    {
        InitMenu();
        Table = new TableModel<TItem>(UI, Columns, Actions)
        {
            PageName = Name,
            ShowCheckBox = Tools != null && Tools.Count > 0,
            Templates = [],
            OnQuery = OnQueryAsync,
            OnAction = OnActionClick
        };
        return base.OnInitPageAsync();
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        UI.BuildPage(builder, new PageModel<TItem>
        {
            Tools = Tools,
            OnToolClick = OnToolClick,
            Table = Table,
            Tree = Tree
        });
    }

    protected virtual Task<PagingResult<TItem>> OnQueryAsync(PagingCriteria criteria) => Task.FromResult(new PagingResult<TItem>());

    protected async void ShowImportForm(string param = null)
    {
        var type = typeof(TItem);
        var id = $"{type.Name}Import";
        if (!string.IsNullOrWhiteSpace(param))
            id += $"_{param}";
        var info = await Platform.File.GetImportAsync(id);
        info.Name = Name;
        Table.ImportForm(info);
    }

    private void OnToolClick(ActionInfo info) => OnAction(info, null);
    private void OnActionClick(ActionInfo info, TItem item) => OnAction(info, [item]);

    private void OnAction(ActionInfo info, object[] parameters)
    {
        var type = GetType();
        var method = type.GetMethod(info.Id);
        if (method == null)
            UI.Error($"{info.Name}【{type.Name}.{info.Id}】方法不存在！");
        else
            method.Invoke(this, parameters);
    }

    private void InitMenu()
    {
        if (Context == null || Context.UserMenus == null)
            return;

        var menu = Context.UserMenus.FirstOrDefault(m => m.Id == PageId);
        if (menu == null)
            return;

        Id = menu.Id;
        Name = menu.Name;
        if (menu.Buttons != null && menu.Buttons.Count > 0)
            Tools = menu.Buttons.Select(n => new ActionInfo(n)).ToList();
        if (menu.Actions != null && menu.Actions.Count > 0)
            Actions = menu.Actions.Select(n => new ActionInfo(n)).ToList();
        Columns = menu.Columns;
    }
}