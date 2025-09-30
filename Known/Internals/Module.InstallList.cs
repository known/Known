namespace Known.Internals;

class ModuleInstallList : BaseTablePage<ModuleInfo>
{
    private IModuleService Service;
    [Parameter] public List<ModuleInfo> Modules { get; set; }

    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();
        Service = await CreateServiceAsync<IModuleService>();

        Table.Name = Language.NoInstallModule;
        Table.AutoHeight = false;
        Table.EnableSort = false;
        Table.EnableFilter = false;
        Table.ShowPager = true;
        Table.SelectType = TableSelectType.Checkbox;
        Table.OnQuery = Service.QueryModulesAsync;

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
            var result = await Service.InstallModulesAsync(rows);
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