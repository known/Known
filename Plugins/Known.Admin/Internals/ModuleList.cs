namespace Known.Internals;

/// <summary>
/// 系统模块管理页面组件类。
/// </summary>
class ModuleList : BasePage<SysModule>
{
    private IModuleService Service;
    private List<SysModule> modules;
    private MenuInfo current;
    private int total;
    private TreeModel tree;
    private TableModel<SysModule> table;

    /// <summary>
    /// 异步初始化页面。
    /// </summary>
    /// <returns></returns>
    protected override async Task OnPageInitAsync()
    {
        await base.OnPageInitAsync();
        Service = await CreateServiceAsync<IModuleService>();

        Page.Type = PageType.Column;
        Page.Spans = "28";
        Page.AddItem("kui-card kui-p10", BuildTree);
        Page.AddItem(BuildTable);

        tree = new TreeModel
        {
            ExpandRoot = true,
            OnNodeClick = OnNodeClickAsync,
            OnModelChanged = OnTreeModelChanged
        };

        table = new TableModel<SysModule>(this)
        {
            FormType = typeof(ModuleForm),
            FormTitle = row => $"{Language["Menu.SysModuleList"]} - {row.ParentName} > {row.Name}",
            Form = new FormInfo { Width = 1200, Maximizable = true, ShowFooter = true },
            RowKey = r => r.Id,
            ShowPager = false,
            SelectType = TableSelectType.Checkbox,
            OnQuery = OnQueryModulesAsync
        };

        table.Toolbar.ShowCount = 6;
        table.Toolbar.AddAction(nameof(New));
        table.Toolbar.AddAction(nameof(DeleteM));
        table.Toolbar.AddAction(nameof(Copy));
        table.Toolbar.AddAction(nameof(Move));
        table.Toolbar.AddAction(nameof(Import));
        table.Toolbar.AddAction(nameof(Export));

        table.AddColumn(c => c.Code).Width(130).ViewLink();
        table.AddColumn(c => c.Name).Width(120).Template(BuildName);
        table.AddColumn(c => c.Description).Width(180);
        table.AddColumn(c => c.Target).Width(80).Template((b, r) => b.Tag(r.Target));
        table.AddColumn(c => c.Url).Width(150);
        table.AddColumn(c => c.Sort).Width(60).Align("center");
        table.AddColumn(c => c.Enabled).Width(60).Align("center");
        table.AddColumn(c => c.Note).Width(150);

        table.AddAction(nameof(Edit));
        table.AddAction(nameof(Delete));
        table.AddAction(nameof(MoveUp));
        table.AddAction(nameof(MoveDown));
    }

    /// <summary>
    /// 页面呈现后，调和后台数据。
    /// </summary>
    /// <param name="firstRender">是否首次呈现。</param>
    /// <returns></returns>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
            await tree.RefreshAsync();
    }

    /// <summary>
    /// 异步刷新页面。
    /// </summary>
    /// <returns></returns>
    public override async Task RefreshAsync()
    {
        await tree.RefreshAsync();
        await table.RefreshAsync();
    }

    private void BuildTree(RenderTreeBuilder builder) => builder.Tree(tree);
    private void BuildTable(RenderTreeBuilder builder) => builder.Table(table);

    private void BuildName(RenderTreeBuilder builder, SysModule row)
    {
        builder.Icon(row.Icon);
        builder.Span(row.Name);
    }

    private Task<PagingResult<SysModule>> OnQueryModulesAsync(PagingCriteria criteria)
    {
        var data = current?.Children?.Select(c => (SysModule)c.Data).ToList();
        total = data?.Count ?? 0;
        var result = new PagingResult<SysModule>(data);
        return Task.FromResult(result);
    }

    /// <summary>
    /// 弹出新增表单对话框。
    /// </summary>
    public void New()
    {
        if (current == null)
        {
            UI.Error(Language["Tip.SelectParentModule"]);
            return;
        }

        table.NewForm(Service.SaveModuleAsync, new SysModule { ParentId = current?.Id, ParentName = current?.Name, Sort = total + 1 });
    }

    /// <summary>
    /// 弹出编辑表单对话框。
    /// </summary>
    /// <param name="row">表格行绑定的对象。</param>
    public void Edit(SysModule row) => table.EditForm(Service.SaveModuleAsync, row);

    /// <summary>
    /// 删除一条数据。
    /// </summary>
    /// <param name="row">表格行绑定的对象。</param>
    public void Delete(SysModule row) => table.Delete(Service.DeleteModulesAsync, row);

    /// <summary>
    /// 批量删除多条数据。
    /// </summary>
    public void DeleteM() => table.DeleteM(Service.DeleteModulesAsync);

    /// <summary>
    /// 复制多个模块到另一个模块下面。
    /// </summary>
    public void Copy() => table.SelectRows(OnCopy);

    /// <summary>
    /// 移动多个模块到另一个模块下面。
    /// </summary>
    public void Move() => table.SelectRows(OnMove);

    /// <summary>
    /// 上移一个模块。
    /// </summary>
    /// <param name="row">表格行绑定的对象。</param>
    public Task MoveUp(SysModule row) => OnMoveAsync(row, true);

    /// <summary>
    /// 下移一个模块。
    /// </summary>
    /// <param name="row">表格行绑定的对象。</param>
    public Task MoveDown(SysModule row) => OnMoveAsync(row, false);

    /// <summary>
    /// 导入模块数据。
    /// </summary>
    public void Import()
    {
        var form = new FormModel<FileFormInfo>(this)
        {
            Title = Language.GetImportTitle(PageName),
            ConfirmText = Language["Tip.ImportModules"],
            Data = new FileFormInfo(),
            OnSaveFile = Service.ImportModulesAsync,
            OnSaved = async d => await RefreshAsync()
        };
        form.AddRow().AddColumn(Language["Import.File"], c => c.BizType, c => c.Type = FieldType.File);
        UI.ShowForm(form);
    }

    /// <summary>
    /// 导出模块数据。
    /// </summary>
    public Task Export()
    {
        return App?.ShowSpinAsync(Language["Tip.DataExporting"], async () =>
        {
            var info = await Service.ExportModulesAsync();
            if (info != null && info.Bytes != null && info.Bytes.Length > 0)
            {
                var stream = new MemoryStream(info.Bytes);
                await JS.DownloadFileAsync(info.Name, stream);
            }
        });
    }

    private void OnCopy(List<SysModule> rows)
    {
        ShowTreeModal(Language["Title.CopyTo"], node =>
        {
            rows.ForEach(m => m.ParentId = node.Id);
            return Service.CopyModulesAsync(rows);
        });
    }

    private void OnMove(List<SysModule> rows)
    {
        ShowTreeModal(Language["Title.MoveTo"], node =>
        {
            rows.ForEach(m => m.ParentId = node.Id);
            return Service.MoveModulesAsync(rows);
        });
    }

    private async Task OnMoveAsync(SysModule row, bool isMoveUp)
    {
        row.IsMoveUp = isMoveUp;
        var result = await Service.MoveModuleAsync(row);
        UI.Result(result, RefreshAsync);
    }

    private async Task OnNodeClickAsync(MenuInfo item)
    {
        current = item;
        await table.RefreshAsync();
    }

    private async Task<TreeModel> OnTreeModelChanged()
    {
        modules = await Service.GetModulesAsync();
        if (modules != null && modules.Count > 0)
        {
            tree.Data = modules.ToMenuItems(ref current);
            tree.SelectedKeys = [current.Id];
            await table.RefreshAsync();
        }
        DataHelper.Initialize(modules?.ToModuleLists());
        return tree;
    }

    private void ShowTreeModal(string title, Func<SysModule, Task<Result>> action)
    {
        SysModule node = null;
        var model = new DialogModel
        {
            Title = title,
            Content = builder =>
            {
                builder.Tree(new TreeModel
                {
                    ExpandRoot = true,
                    Data = modules.ToMenuItems(),
                    OnNodeClick = n =>
                    {
                        node = n.Data as SysModule;
                        return Task.CompletedTask;
                    }
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