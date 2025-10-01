namespace Known.Internals;

class ModuleInstallList : BaseTable<SysModule>
{
    private IModuleService Service;
    [Parameter] public List<MenuInfo> Menus { get; set; }

    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        Service = await CreateServiceAsync<IModuleService>();

        //Table.Name = string.Empty;//Language.NoInstallModule;
        Table.EnableSort = false;
        Table.EnableFilter = false;
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
    public void Install(SysModule row) => ShowInstallTree([row]);

    private void BuildName(RenderTreeBuilder builder, SysModule row)
    {
        builder.IconName(row.Icon, row.Name);
    }

    private void ShowInstallTree(List<SysModule> rows)
    {
        SysModule node = null;
        var model = new DialogModel
        {
            Title = Language.InstallTo,
            Content = builder =>
            {
                builder.Tree(new TreeModel
                {
                    ExpandRoot = true,
                    Data = Menus.ToMenuItems(),
                    OnNodeClick = n =>
                    {
                        node = n.DataAs<SysModule>();
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
                rows.ForEach(m => Menus.Add(m.ToMenuInfo()));
                await model.CloseAsync();
                await RefreshAsync();
            });
        };
        UI.ShowDialog(model);
    }
}