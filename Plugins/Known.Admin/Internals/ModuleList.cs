namespace Known.Internals;

class ModuleList : BaseTablePage<SysModule1>
{
    private IModule1Service Service;
    private List<SysModule1> modules;
    private MenuInfo current;
    private int total;
    private TreeModel Tree;

    protected override async Task OnInitPageAsync()
    {
        if (!CurrentUser.IsSystemAdmin())
        {
            Navigation.GoErrorPage("403");
            return;
        }

        await base.OnInitPageAsync();
        Service = await CreateServiceAsync<IModule1Service>();

        Tree = new TreeModel
        {
            ExpandRoot = true,
            OnNodeClick = OnNodeClickAsync,
            OnModelChanged = OnTreeModelChanged
        };

        Table = new TableModel<SysModule1>(this)
        {
            FormType = typeof(ModuleForm),
            FormTitle = row => $"{Language["Menu.SysModuleList"]} - {row.ParentName} > {row.Name}",
            Form = new FormInfo { Width = 1200, Maximizable = true, ShowFooter = true },
            //RowKey = r => r.Id,
            ShowPager = false,
            SelectType = TableSelectType.Checkbox,
            OnQuery = OnQueryModulesAsync
        };

        Table.Toolbar.ShowCount = 6;
        Table.Toolbar.AddAction(nameof(New));
        Table.Toolbar.AddAction(nameof(DeleteM));
        Table.Toolbar.AddAction(nameof(Copy));
        Table.Toolbar.AddAction(nameof(Move));
        Table.Toolbar.AddAction(nameof(Import));
        Table.Toolbar.AddAction(nameof(Export));

        Table.AddColumn(c => c.Code).Width(130).ViewLink();
        Table.AddColumn(c => c.Name).Width(120).Template(BuildName);
        Table.AddColumn(c => c.Description).Width(180);
        Table.AddColumn(c => c.Target).Width(80).Tag();
        Table.AddColumn(c => c.Url).Width(150);
        Table.AddColumn(c => c.Sort).Width(60).Center();
        Table.AddColumn(c => c.Enabled).Width(60).Center();
        Table.AddColumn(c => c.Note).Width(150);

        Table.ActionCount = 4;
        Table.ActionWidth = "200";
        Table.AddAction(nameof(Edit));
        Table.AddAction(nameof(Delete));
        Table.AddAction(nameof(MoveUp));
        Table.AddAction(nameof(MoveDown));
    }

    protected override void BuildPage(RenderTreeBuilder builder)
    {
        builder.Component<KTreeTable<SysModule1>>()
               .Set(c => c.Tree, Tree)
               .Set(c => c.Table, Table)
               .Build();
    }

    public override async Task RefreshAsync()
    {
        await Tree.RefreshAsync();
        await Table.RefreshAsync();
    }

    private void BuildName(RenderTreeBuilder builder, SysModule1 row)
    {
        builder.IconName(row.Icon, row.Name);
    }

    private Task<PagingResult<SysModule1>> OnQueryModulesAsync(PagingCriteria criteria)
    {
        var data = current?.Children?.Select(c => c.DataAs<SysModule1>()).ToList();
        total = data?.Count ?? 0;
        var result = new PagingResult<SysModule1>(data);
        return Task.FromResult(result);
    }

    public void New()
    {
        if (current == null)
        {
            UI.Error(Language["Tip.SelectParentModule"]);
            return;
        }

        Table.NewForm(Service.SaveModuleAsync, new SysModule1 { ParentId = current?.Id, ParentName = current?.Name, Sort = total + 1 });
    }

    public void Edit(SysModule1 row) => Table.EditForm(Service.SaveModuleAsync, row);
    public void Delete(SysModule1 row) => Table.Delete(Service.DeleteModulesAsync, row);
    public void DeleteM() => Table.DeleteM(Service.DeleteModulesAsync);
    public void Copy() => Table.SelectRows(OnCopy);
    public void Move() => Table.SelectRows(OnMove);
    public Task MoveUp(SysModule1 row) => OnMoveAsync(row, true);
    public Task MoveDown(SysModule1 row) => OnMoveAsync(row, false);

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

    public Task Export()
    {
        return App?.ShowSpinAsync(Language["Tip.DataExporting"], async () =>
        {
            var info = await Service.ExportModulesAsync();
            await JS.DownloadFileAsync(info);
        });
    }

    private void OnCopy(List<SysModule1> rows)
    {
        ShowTreeModal(Language["Title.CopyTo"], node =>
        {
            rows.ForEach(m => m.ParentId = node.Id);
            return Service.CopyModulesAsync(rows);
        });
    }

    private void OnMove(List<SysModule1> rows)
    {
        ShowTreeModal(Language["Title.MoveTo"], node =>
        {
            rows.ForEach(m => m.ParentId = node.Id);
            return Service.MoveModulesAsync(rows);
        });
    }

    private async Task OnMoveAsync(SysModule1 row, bool isMoveUp)
    {
        row.IsMoveUp = isMoveUp;
        var result = await Service.MoveModuleAsync(row);
        UI.Result(result, RefreshAsync);
    }

    private async Task OnNodeClickAsync(MenuInfo item)
    {
        current = item;
        await Table.RefreshAsync();
    }

    private async Task<TreeModel> OnTreeModelChanged()
    {
        modules = await Service.GetModulesAsync();
        if (modules != null && modules.Count > 0)
        {
            Tree.Data = modules.ToMenuItems(ref current);
            Tree.SelectedKeys = [current.Id];
            await Table.RefreshAsync();
        }
        return Tree;
    }

    private void ShowTreeModal(string title, Func<SysModule1, Task<Result>> action)
    {
        SysModule1 node = null;
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
                        node = n.DataAs<SysModule1>();
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