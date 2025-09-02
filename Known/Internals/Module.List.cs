namespace Known.Internals;

class ModuleList : BasePage<ModuleInfo>
{
    private List<ModuleInfo> modules;
    private MenuInfo current;
    private int total;
    private TreeModel Tree;
    private TableModel<ModuleInfo> Table;

    /// <summary>
    /// 异步初始化页面。
    /// </summary>
    /// <returns></returns>
    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();

        Page.Type = PageType.Column;
        Page.Spans = "28";
        Page.AddItem("kui-card kui-p10", BuildTree);
        Page.AddItem(BuildTable);

        Tree = new TreeModel
        {
            ExpandRoot = true,
            OnNodeClick = OnNodeClickAsync,
            OnModelChanged = OnTreeModelChanged
        };

        Table = new TableModel<ModuleInfo>(this)
        {
            FormType = typeof(ModuleForm),
            FormTitle = row => $"{Language[Language.SysModule]} - {row.ParentName} > {row.Name}",
            Form = new FormInfo { Width = 800, Maximizable = true, ShowFooter = true },
            EnableEdit = false,
            EnableFilter = false,
            ShowPager = false,
            SelectType = TableSelectType.Checkbox,
            OnQuery = OnQueryModulesAsync
        };

        Table.Toolbar.ShowCount = 10;
        Table.Toolbar.AddAction(nameof(New));
        Table.Toolbar.AddAction(nameof(DeleteM));
        Table.Toolbar.AddAction(nameof(Copy));
        Table.Toolbar.AddAction(nameof(Move));
        Table.Toolbar.AddAction(nameof(Import));
        Table.Toolbar.AddAction(nameof(Export));
        Table.Toolbar.AddAction(nameof(Install), Language.TipNewModule);
        Table.Toolbar.AddAction(nameof(Migrate), Language.MigrateModule);

        Table.AddColumn(c => c.Name).Width(120).Template(BuildName);
        Table.AddColumn(c => c.Type).Width(80).Tag();
        Table.AddColumn(c => c.Url);
        Table.AddColumn(c => c.Target).Width(100).Tag();
        Table.AddColumn(c => c.Sort).Width(60).Align("center");
        Table.AddColumn(c => c.Enabled).Width(60).Align("center");

        Table.ActionCount = 4;
        Table.ActionWidth = "200";
        Table.AddAction(nameof(Edit));
        Table.AddAction(nameof(Delete));
        Table.AddAction(nameof(MoveUp));
        Table.AddAction(nameof(MoveDown));
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
            await Tree.RefreshAsync();
    }

    /// <summary>
    /// 异步刷新页面。
    /// </summary>
    /// <returns></returns>
    public override async Task RefreshAsync()
    {
        await Tree.RefreshAsync();
        await Table.RefreshAsync();
    }

    private void BuildTree(RenderTreeBuilder builder) => builder.Tree(Tree);
    private void BuildTable(RenderTreeBuilder builder) => builder.PageTable(Table);

    private void BuildName(RenderTreeBuilder builder, ModuleInfo row)
    {
        builder.IconName(row.Icon, row.Name);
    }

    private Task<PagingResult<ModuleInfo>> OnQueryModulesAsync(PagingCriteria criteria)
    {
        var data = current?.Children?.Select(c => (ModuleInfo)c.Data).ToList();
        total = data?.Count ?? 0;
        var result = new PagingResult<ModuleInfo>(data);
        return Task.FromResult(result);
    }

    /// <summary>
    /// 弹出新增表单对话框。
    /// </summary>
    public void New()
    {
        if (current == null)
        {
            UI.Error(Language.TipSelectParentModule);
            return;
        }

        Table.NewForm(Platform.SaveModuleAsync, new ModuleInfo
        {
            ParentId = current?.Id,
            ParentName = current?.Name,
            Type = nameof(MenuType.Menu),
            Target = nameof(LinkTarget.None),
            Sort = total + 1
        });
    }

    /// <summary>
    /// 弹出编辑表单对话框。
    /// </summary>
    /// <param name="row">表格行绑定的对象。</param>
    public void Edit(ModuleInfo row)
    {
        if (row.IsCode)
        {
            UI.Error(Language.TipCodeModuleNotOperate);
            return;
        }

        Table.EditForm(Platform.SaveModuleAsync, row);
    }

    /// <summary>
    /// 删除一条数据。
    /// </summary>
    /// <param name="row">表格行绑定的对象。</param>
    public void Delete(ModuleInfo row)
    {
        if (row.IsCode)
        {
            UI.Error(Language.TipCodeModuleNotOperate);
            return;
        }

        Table.Delete(Platform.DeleteModulesAsync, row);
    }

    /// <summary>
    /// 批量删除多条数据。
    /// </summary>
    public void DeleteM() => Table.DeleteM(Platform.DeleteModulesAsync);

    /// <summary>
    /// 复制多个模块到另一个模块下面。
    /// </summary>
    public void Copy() => Table.SelectRows(OnCopy);

    /// <summary>
    /// 移动多个模块到另一个模块下面。
    /// </summary>
    public void Move() => Table.SelectRows(OnMove);

    /// <summary>
    /// 上移一个模块。
    /// </summary>
    /// <param name="row">表格行绑定的对象。</param>
    public Task MoveUp(ModuleInfo row) => OnMoveAsync(row, true);

    /// <summary>
    /// 下移一个模块。
    /// </summary>
    /// <param name="row">表格行绑定的对象。</param>
    public Task MoveDown(ModuleInfo row) => OnMoveAsync(row, false);

    /// <summary>
    /// 导入模块数据。
    /// </summary>
    public void Import()
    {
        var form = new FormModel<FileFormInfo>(this)
        {
            Title = Language.GetImportTitle(PageName),
            ConfirmText = Language.TipImportModules,
            Data = new FileFormInfo(),
            OnSaveFile = Platform.ImportModulesAsync,
            OnSaved = async d => await RefreshAsync()
        };
        form.AddRow().AddColumn(Language.ImportFile, c => c.BizType, c => c.Type = FieldType.File);
        UI.ShowForm(form);
    }

    /// <summary>
    /// 导出模块数据。
    /// </summary>
    public Task Export()
    {
        return App?.ShowSpinAsync(Language.DataExporting, async () =>
        {
            var info = await Platform.ExportModulesAsync();
            await JS.DownloadFileAsync(info);
        });
    }

    /// <summary>
    /// 安装新模块数据。
    /// </summary>
    public void Install()
    {
        var model = new DialogModel
        {
            Title = Language.InstallNewModule,
            Width = 800,
            Content = b => b.Component<ModuleInstallList>().Set(c => c.Modules, modules).Build()
        };
        model.OnOk = async () =>
        {
            await model.CloseAsync();
            await RefreshAsync();
        };
        UI.ShowDialog(model);
    }

    /// <summary>
    /// 迁移配置数据。
    /// </summary>
    public void Migrate()
    {
        UI.Confirm(Language.ConfirmMigrate, async () =>
        {
            var result = await Platform.MigrateModulesAsync();
            UI.Result(result, RefreshAsync);
        });
    }

    private void OnCopy(List<ModuleInfo> rows)
    {
        ShowTreeModal(Language.CopyTo, node =>
        {
            rows.ForEach(m => m.ParentId = node.Id);
            return Platform.CopyModulesAsync(rows);
        });
    }

    private void OnMove(List<ModuleInfo> rows)
    {
        ShowTreeModal(Language.MoveTo, node =>
        {
            rows.ForEach(m => m.ParentId = node.Id);
            return Platform.MoveModulesAsync(rows);
        });
    }

    private async Task OnMoveAsync(ModuleInfo row, bool isMoveUp)
    {
        if (row.IsCode)
        {
            UI.Error(Language.TipCodeModuleNotOperate);
            return;
        }

        row.IsMoveUp = isMoveUp;
        var result = await Platform.MoveModuleAsync(row);
        UI.Result(result, RefreshAsync);
    }

    private async Task OnNodeClickAsync(MenuInfo item)
    {
        current = item;
        await Table.RefreshAsync();
    }

    private async Task<TreeModel> OnTreeModelChanged()
    {
        modules = await Platform.GetModulesAsync();
        if (modules != null && modules.Count > 0)
        {
            Tree.Data = modules.ToMenuItems(ref current);
            Tree.SelectedKeys = [current.Id];
            await Table.RefreshAsync();
        }
        return Tree;
    }

    private void ShowTreeModal(string title, Func<ModuleInfo, Task<Result>> action)
    {
        ModuleInfo node = null;
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
                        node = n.Data as ModuleInfo;
                        return Task.CompletedTask;
                    }
                });
            }
        };
        model.OnOk = async () =>
        {
            if (node == null)
            {
                UI.Error(Language.TipSelectModule);
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