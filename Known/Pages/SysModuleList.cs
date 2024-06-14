namespace Known.Pages;

//[Authorize]
[StreamRendering]
[Route("/sys/modules")]
public class SysModuleList : BasePage<SysModule>
{
    private IModuleService moduleService; 
    private List<SysModule> modules;
    private MenuInfo current;
    private int total;
    private TreeModel tree;
    private TableModel<SysModule> table;

    protected override async Task OnPageInitAsync()
    {
        await base.OnPageInitAsync();
        moduleService = await CreateServiceAsync<IModuleService>();

        Page.Type = PageType.Column;
        Page.Spans = "28";
        Page.AddItem("kui-card", BuildTree);
        Page.AddItem(BuildTable);

        tree = new TreeModel
        {
            ExpandRoot = true,
            OnNodeClick = OnNodeClick,
            OnModelChanged = OnTreeModelChanged
        };

        table = new TableModel<SysModule>(this)
        {
            FormType = typeof(ModuleForm),
            FormTitle = row => $"{PageName} - {row.ParentName} > {row.Name}",
            RowKey = r => r.Id,
            ShowPager = false,
            OnQuery = OnQueryModulesAsync
        };
    }

    protected override async Task OnPageChangeAsync()
    {
        table.Initialize(this);
        await base.OnPageChangeAsync();
        table.Column(c => c.Name).Template(BuildName);
        table.Column(c => c.Target).Template(BuildTarget);

        tree.Load();
    }

    public override async Task RefreshAsync()
    {
        await tree.RefreshAsync();
        await table.RefreshAsync();
    }

    private void BuildTree(RenderTreeBuilder builder) => builder.Div("p10", () => UI.BuildTree(builder, tree));
    private void BuildTable(RenderTreeBuilder builder) => builder.BuildTable(table);

    private void BuildName(RenderTreeBuilder builder, SysModule row)
    {
        UI.Icon(builder, row.Icon);
        builder.Span(row.Name);
    }

    private void BuildTarget(RenderTreeBuilder builder, SysModule row) => UI.BuildTag(builder, row.Target);

    private Task<PagingResult<SysModule>> OnQueryModulesAsync(PagingCriteria criteria)
    {
        var data = current?.Children?.Select(c => (SysModule)c.Data).ToList();
        total = data?.Count ?? 0;
        var result = new PagingResult<SysModule>(data);
        return Task.FromResult(result);
    }

    public void New()
    {
        if (current == null)
        {
            UI.Error(Language["Tip.SelectParentModule"]);
            return;
        }

        table.NewForm(moduleService.SaveModuleAsync, new SysModule { ParentId = current?.Id, ParentName = current?.Name, Sort = total + 1 });
    }

    public void Edit(SysModule row) => table.EditForm(moduleService.SaveModuleAsync, row);
    public void Delete(SysModule row) => table.Delete(moduleService.DeleteModulesAsync, row);
    public void DeleteM() => table.DeleteM(moduleService.DeleteModulesAsync);

    public void Copy() => table.SelectRows(OnCopy);
    public void Move() => table.SelectRows(OnMove);
    public void MoveUp(SysModule row) => OnMove(row, true);
    public void MoveDown(SysModule row) => OnMove(row, false);

    private void OnCopy(List<SysModule> rows)
    {
        ShowTreeModal(Language["Title.CopyTo"], node =>
        {
            rows.ForEach(m => m.ParentId = node.Id);
            return moduleService.CopyModulesAsync(rows);
        });
    }

    private void OnMove(List<SysModule> rows)
    {
        ShowTreeModal(Language["Title.MoveTo"], node =>
        {
            rows.ForEach(m => m.ParentId = node.Id);
            return moduleService.MoveModulesAsync(rows);
        });
    }

    private async void OnMove(SysModule row, bool isMoveUp)
    {
        row.IsMoveUp = isMoveUp;
        var result = await moduleService.MoveModuleAsync(row);
        UI.Result(result, async () => await RefreshAsync());
    }

    private async void OnNodeClick(MenuInfo item)
    {
        current = item;
        await table.RefreshAsync();
    }

    private async void OnTreeModelChanged(TreeModel model)
    {
        modules = await moduleService.GetModulesAsync();
        if (modules != null && modules.Count > 0)
        {
            tree.Data = modules.ToMenuItems(ref current);
            tree.SelectedKeys = [current?.Id];
        }
        model.Data = tree.Data;
        model.SelectedKeys = tree.SelectedKeys;
        DataHelper.Initialize(modules);
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
                UI.Error(Language["Tip.SelectModule"]);
                return;
            }

            var result = await action?.Invoke(node);
            UI.Result(result, async () =>
            {
                await model.CloseAsync();
                await RefreshAsync();
            });
        };
        UI.ShowDialog(model);
    }
}