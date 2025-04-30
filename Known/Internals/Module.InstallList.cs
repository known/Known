namespace Known.Internals;

class ModuleInstallList : BaseTablePage<ModuleInfo>
{
    [Parameter] public List<ModuleInfo> Modules { get; set; }

    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();
        Table.Name = Language.NoInstallModule;
        Table.AutoHeight = false;
        Table.EnableSort = false;
        Table.EnableFilter = false;
        Table.ShowPager = true;
        Table.SelectType = TableSelectType.Checkbox;
        Table.OnQuery = OnQueryModulesAsync;

        Table.AddColumn(c => c.Name, true).Width(120).Template(BuildName);
        Table.AddColumn(c => c.Type).Width(80).Tag();
        Table.AddColumn(c => c.Url).Width(150);
        Table.AddColumn(c => c.Target).Width(100).Tag();
        Table.AddColumn(c => c.Sort).Width(60).Align("center");
        Table.AddColumn(c => c.Enabled).Width(60).Align("center");

        Table.Toolbar.AddAction(nameof(Install));
        Table.AddAction(nameof(Install));
    }

    public void Install() => Table.SelectRows(ShowInstallTree);
    public void Install(ModuleInfo row) => ShowInstallTree([row]);

    private async Task<PagingResult<ModuleInfo>> OnQueryModulesAsync(PagingCriteria criteria)
    {
        var modules = AppData.Data.Modules.Where(m => Modules?.Exists(d => d.Url == m.Url) == false)
                                          .OrderBy(d => d.ParentId)
                                          .ThenBy(d => d.Sort)
                                          .ToList();
        var name = criteria.GetQueryValue(nameof(ModuleInfo.Name));
        if (!string.IsNullOrEmpty(name))
            modules = [.. modules.Where(m => m.Name.Contains(name))];

        var result = modules.ToPagingResult(criteria);
        return await Task.FromResult(result);
    }

    private void BuildName(RenderTreeBuilder builder, ModuleInfo row)
    {
        builder.IconName(row.Icon, row.Name);
    }

    private void ShowInstallTree(List<ModuleInfo> rows)
    {
        ModuleInfo node = null;
        var model = new DialogModel
        {
            Title = Language.InstallTo,
            Content = builder =>
            {
                builder.Tree(new TreeModel
                {
                    ExpandRoot = true,
                    Data = Modules.ToMenuItems(),
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

            rows.ForEach(m => m.ParentId = node.Id);
            var result = await Platform.InstallModulesAsync(rows);
            UI.Result(result, async () =>
            {
                rows.ForEach(m => Modules.Add(m));
                await model.CloseAsync();
                await RefreshAsync();
            });
        };
        UI.ShowDialog(model);
    }
}