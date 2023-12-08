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
	private TablePageModel<SysModule> table;

	protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();

		Page.Type = PageType.Column;
		Page.Spans = [4, 20];
		Page.Contents = [BuildTree, BuildTable];

		tree = new TreeModel
		{
			ExpandParent = true,
			OnNodeClick = OnNodeClick,
			OnRefresh = OnTreeRefresh
		};

		table = new TablePageModel<SysModule>(this)
		{
			FormTitle = row => $"{Name} - {row.ParentName}",
			RowKey = r => r.Id,
			ShowPager = false,
			OnQuery = OnQueryModulesAsync,
			OnToolClick = OnToolClick,
			OnAction = OnActionClick
		};
		table.Form.Width = 1000;
		table.Form.Maximizable = true;
		table.Form.NoFooter = true;

		await OnTreeRefresh();
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
		//TODO：保存删除时，左侧树刷新问题
		await tree.RefreshAsync();
		//model.StateChanged.Invoke();
		await table.RefreshAsync();
		//StateChanged();
	}

	private void BuildTree(RenderTreeBuilder builder) => builder.Div("p10", () => UI.BuildTree(builder, tree));
	private void BuildTable(RenderTreeBuilder builder) => UI.BuildPage(builder, table);

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

    private async Task OnTreeRefresh()
    {
        modules = await Platform.Module.GetModulesAsync();
        tree.Data = modules.ToMenuItems(ref current);
        if (current == null || current.Id == "0")
            current = tree.Data[0];
        tree.SelectedKeys = [current?.Id];
    }

    private void ShowTreeModal(string title, Func<SysModule, Task<Result>> action)
    {
        SysModule node = null;
        var option = new ModalOption
        {
            Title = title,
            Content = builder =>
            {
                UI.BuildTree(builder, new TreeModel
                {
                    ExpandParent = true,
                    Data = modules.ToMenuItems(),
                    OnNodeClick = n => node = n.Data as SysModule
                });
            }
        };
        option.OnOk = async () =>
        {
            if (node == null)
            {
                UI.Error("请选择模块！");
                return;
            }

            var result = await action?.Invoke(node);
            UI.Result(result, async () =>
            {
                await option.OnClose?.Invoke();
                await RefreshAsync();
            });
        };
        UI.ShowModal(option);
    }
}

class SysModuleForm : BaseForm<SysModule>
{
	private readonly StepModel step = new() { IsContent = true };

	private List<CodeInfo> ModelTypes { get; set; }

	protected override async Task OnInitFormAsync()
	{
		await base.OnInitFormAsync();
        ModelTypes = Config.ModelTypes.Select(m => new CodeInfo(m.Name, m.Name)).ToList();

		step.Items.Add(new("基本信息") { Content = BuildDataForm });
		step.Items.Add(new("页面设置") { Content = BuildModulePage });//TODO：模块页面和表单配置组件开发
		step.Items.Add(new("表单设置") { Content = BuildModuleForm });
		step.IsView = Model.IsView;
		step.OnSave = SaveAsync;
	}

	protected override void BuildRenderTree(RenderTreeBuilder builder) => builder.Cascading<SysModuleForm>(this, BuildForm);

	private void BuildForm(RenderTreeBuilder builder) => UI.BuildSteps(builder, step);
	private void BuildDataForm(RenderTreeBuilder builder) => UI.BuildForm(builder, Model);

	private void BuildModulePage(RenderTreeBuilder builder)
	{
        builder.Div("module-page", () =>
        {
            builder.Div("left", () =>
            {
                builder.Div("", () =>
                {
                    builder.Span("实体类型");
                    UI.BuildInput(builder, new InputOption<string> { ValueChanged = Callback<string>(OnModelChanged) });
                });
            });
			builder.Div("right", () =>
			{

			});
		});
	}

	private void OnModelChanged(string obj)
	{
	}

	private void BuildModuleForm(RenderTreeBuilder builder)
	{
	}

	private async Task<bool> SaveAsync(bool isClose = false)
	{
		if (!Model.Validate())
			return false;

		await Model.SaveAsync(isClose);
		return true;
	}
}