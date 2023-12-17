using Known.Entities;
using Known.Extensions;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Blazor;

class SysModuleList : BasePage<SysModule>
{
    private List<SysModule> modules;
    private MenuItem current;
    private int total;
	private TreeModel tree;
	private TableModel<SysModule> table;

	protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();

        //页面类型，左右布局
		Page.Type = PageType.Column;
		Page.Spans = [4, 20];
		Page.Contents = [BuildTree, BuildTable];

        //左侧模块树模型
		tree = new TreeModel
		{
			ExpandRoot = true,
			OnNodeClick = OnNodeClick,
			OnModelChanged = OnTreeModelChanged
        };
        tree.Load();

        //右侧模块表格模型
        table = new TableModel<SysModule>(this)
		{
			FormTitle = row => $"{Name} - {row.ParentName} > {row.Name}",
			RowKey = r => r.Id,
			ShowPager = false,
			OnQuery = OnQueryModulesAsync,
			OnAction = OnActionClick
		};
        table.Toolbar.OnItemClick = OnToolClick;

        table.Form.Width = 1200;
		table.Form.Maximizable = true;
		table.Form.DefaultMaximized = true;
		table.Form.NoFooter = true;

        table.Column(c => c.Name).Template(BuildName);
        table.Column(c => c.Target).Template(BuildTarget);
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        //if (!Config.App.IsDevelopment)
        //{
        //    UI.BuildResult(builder, "403", "非开发环境，不可访问该页面！");
        //    return;
        //}
        base.BuildRenderTree(builder);
    }

	public override async Task RefreshAsync()
	{
		await tree.RefreshAsync();
		await table.RefreshAsync();
	}

	private void BuildTree(RenderTreeBuilder builder) => builder.Div("p10", () => UI.BuildTree(builder, tree));
	private void BuildTable(RenderTreeBuilder builder) => builder.BuildTablePage(table);

    private void BuildName(RenderTreeBuilder builder, SysModule row)
    {
        UI.BuildIcon(builder, row.Icon);
        builder.Span(row.Name);
    }

    private void BuildTarget(RenderTreeBuilder builder, SysModule row)
    {
        var color = "blue";
        if (row.Target == "菜单") color = "purple";
        if (row.Target == "自定义") color = "green";
        UI.BuildTag(builder, row.Target, color);
    }

    private Task<PagingResult<SysModule>> OnQueryModulesAsync(PagingCriteria criteria)
    {
        var data = current?.Children?.Select(c => (SysModule)c.Data).ToList();
        total = data?.Count ?? 0;
        var result = new PagingResult<SysModule>(data);
        return Task.FromResult(result);
    }

    [Action]
    public void New()
    {
        if (current == null)
        {
            UI.Error("请先选择上级模块！");
            return;
        }

        table.NewForm(Platform.Module.SaveModuleAsync, new SysModule { ParentId = current?.Id, ParentName = current?.Name, Sort = total + 1 });
    }

    [Action] public void Edit(SysModule row) => table.EditForm(Platform.Module.SaveModuleAsync, row);
    [Action] public void Delete(SysModule row) => table.Delete(Platform.Module.DeleteModulesAsync, row);
    [Action] public void DeleteM() => table.DeleteM(Platform.Module.DeleteModulesAsync);

    [Action] public void Copy() => table.SelectRows(OnCopy);
    [Action] public void Move() => table.SelectRows(OnMove);
    [Action] public void MoveUp(SysModule row) => OnMove(row, true);
    [Action] public void MoveDown(SysModule row) => OnMove(row, false);

    private void OnCopy(List<SysModule> rows)
    {
        ShowTreeModal("复制到", node =>
        {
            rows.ForEach(m => m.ParentId = node.Id);
            return Platform.Module.CopyModulesAsync(rows);
        });
    }

    private void OnMove(List<SysModule> rows)
    {
        ShowTreeModal("移动到", node =>
        {
            rows.ForEach(m => m.ParentId = node.Id);
            return Platform.Module.MoveModulesAsync(rows);
        });
    }

    private async void OnMove(SysModule row, bool isMoveUp)
    {
        row.IsMoveUp = isMoveUp;
        var result = await Platform.Module.MoveModuleAsync(row);
        UI.Result(result, async () => await RefreshAsync());
    }

    private async void OnNodeClick(MenuItem item)
    {
        current = item;
        await table.RefreshAsync();
    }

    private async void OnTreeModelChanged(TreeModel model)
    {
        modules = await Platform.Module.GetModulesAsync();
        if (modules != null && modules.Count > 0)
        {
            tree.Data = modules.ToMenuItems(ref current);
            tree.SelectedKeys = [current?.Id];
        }
        model.Data = tree.Data;
        model.SelectedKeys = tree.SelectedKeys;
    }

    private void ShowTreeModal(string title, Func<SysModule, Task<Result>> action)
    {
        SysModule node = null;
        var model = new DialogModel
        {
            Title = title,
            Content = builder =>
            {
                UI.BuildTree(builder, new TreeModel
                {
                    ExpandRoot = true,
                    Data = modules.ToMenuItems(),
                    OnNodeClick = n => node = n.Data as SysModule
                });
            }
        };
        model.OnOk = async () =>
        {
            if (node == null)
            {
                UI.Error("请选择模块！");
                return;
            }

            var result = await action?.Invoke(node);
            UI.Result(result, async () =>
            {
                await model.OnClose?.Invoke();
                await RefreshAsync();
            });
        };
        UI.ShowDialog(model);
    }
}