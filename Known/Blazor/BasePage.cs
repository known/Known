﻿using Known.Designers;
using Known.Entities;
using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Blazor;

public class BasePage : BaseComponent
{
    internal SysModule Module { get; set; }

    [Parameter] public string PageId { get; set; }

    public string PageName => Language.GetString(Module);

    public virtual Task RefreshAsync() => Task.CompletedTask;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        await OnInitPageAsync();
        await AddVisitLogAsync();
    }

	protected virtual async Task OnInitPageAsync()
    {
        if (!string.IsNullOrWhiteSpace(PageId))
            Module = await Platform.Module.GetModuleAsync(PageId);
    }

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
        if (!string.IsNullOrWhiteSpace(PageId))
            Module = await Platform.Module.GetModuleAsync(PageId);
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (Module == null || string.IsNullOrWhiteSpace(Module.EntityData))
            UI.BuildResult(builder, "404", $"{Language["Tip.Page404"]}PageId={PageId}");
        else
            BuildPrototype(builder);
    }

    private void BuildPrototype(RenderTreeBuilder builder)
    {
        builder.Div("kui-designer-tips", Language["Tip.PageTest"]);
        var table = new DemoPageModel(Context);
        table.Module = Module;
        table.Entity = DataHelper.GetEntity(Module.EntityData);
        table.SetPageInfo(Module.Page);
        builder.BuildTablePage(table);
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

	protected override void BuildRenderTree(RenderTreeBuilder builder) => UI.BuildPage(builder, Page);

	protected void OnToolClick(ActionInfo info) => OnAction(info, null);
	protected void OnActionClick(ActionInfo info, TItem item) => OnAction(info, [item]);

    private void OnAction(ActionInfo info, object[] parameters)
    {
        var type = GetType();
        var method = type.GetMethod(info.Id);
        if (method != null)
        {
            method.Invoke(this, parameters);
            return;
        }

        var message = Language["Tip.NoMethod"].Replace("{method}", $"{info.Name}[{type.Name}.{info.Id}]");
        UI.Error(message);
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

    protected override void BuildRenderTree(RenderTreeBuilder builder) => builder.BuildTablePage(Table);

    protected async void ShowImportForm(string param = null)
	{
		var type = typeof(TItem);
		var id = $"{type.Name}Import";
		if (!string.IsNullOrWhiteSpace(param))
			id += $"_{param}";
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
}

public class BaseTabPage : BasePage
{
    protected TabModel Tab { get; } = new();

    protected override void BuildRenderTree(RenderTreeBuilder builder) => UI.BuildTabs(builder, Tab);
}