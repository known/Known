using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Blazor;

public class BasePage : BaseComponent
{
	[Parameter] public string PageId { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await OnInitPageAsync();
        await AddVisitLogAsync();
        await base.OnInitializedAsync();
    }

	public virtual Task RefreshAsync() => Task.CompletedTask;
	protected virtual Task OnInitPageAsync() => Task.CompletedTask;

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        UI.BuildResult(builder, "404", $"页面不存在！PageId={PageId}");
    }
}

public class BasePage<TItem> : BasePage where TItem : class, new()
{
    protected PageModel Page { get; } = new();
	internal List<ActionInfo> Tools { get; set; }
    internal List<ActionInfo> Actions { get; set; }
    internal List<PageColumnInfo> Columns { get; set; }

    internal virtual void ViewForm(FormType type, TItem row) { }

	protected override Task OnInitPageAsync()
    {
        InitMenu();
        return base.OnInitPageAsync();
    }

	protected override void BuildRenderTree(RenderTreeBuilder builder) => UI.BuildPage(builder, Page);

	protected void OnToolClick(ActionInfo info) => OnAction(info, null);
	protected void OnActionClick(ActionInfo info, TItem item) => OnAction(info, [item]);

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

    protected override void BuildRenderTree(RenderTreeBuilder builder) => builder.BuildTablePage(Table);

    protected async void ShowImportForm(string param = null)
	{
		var type = typeof(TItem);
		var id = $"{type.Name}Import";
		if (!string.IsNullOrWhiteSpace(param))
			id += $"_{param}";
		var info = await Platform.File.GetImportAsync(id);
		info.Name = Name;
		info.BizName = $"导入{Name}";
		ImportForm(info);
	}

    private void ImportForm(ImportFormInfo info)
    {
        var model = new DialogModel { Title = $"导入{Name}" };
        model.Content = builder =>
        {
            builder.Component<Importer>()
                   .Set(c => c.Model, info)
                   .Set(c => c.OnSuccess, async () =>
                   {
                       model.OnClose?.Invoke();
                       await RefreshAsync();
                   })
                   .Build();
        };
        UI.ShowDialog(model);
    }
}

public class BaseTabPage : BasePage
{
    protected TabModel Tab { get; } = new();

	protected override void BuildRenderTree(RenderTreeBuilder builder) => UI.BuildTabs(builder, Tab);
}