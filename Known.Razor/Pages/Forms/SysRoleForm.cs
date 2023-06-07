namespace Known.Razor.Pages.Forms;

[Dialog(1000, 500)]
class SysRoleForm : BaseForm<SysRole>
{
    private readonly Dictionary<string, string> btnValues = new();
    private readonly Dictionary<string, string> colValues = new();
    private HashSet<MenuInfo> values = new();
    private RoleFormInfo info;
    private List<TreeItem<MenuInfo>> data;
    private TreeItem<MenuInfo> curItem;

    protected override async Task InitFormAsync()
    {
        info = await Platform.Role.GetRoleAsync(TModel.Id);
        foreach (var item in info.Menus)
        {
            btnValues[item.Id] = string.Join(",", info.MenuIds.Where(m => m.StartsWith($"b_{item.Id}_")));
            colValues[item.Id] = string.Join(",", info.MenuIds.Where(m => m.StartsWith($"c_{item.Id}_")));
        }
        values = info.Menus.Where(m => info.MenuIds.Contains(m.Id)).ToHashSet();
        data = info.Menus.ToTreeItems();
    }

    protected override void BuildFields(FieldBuilder<SysRole> builder)
    {
        builder.Div("left", attr =>
        {
            attr.Style("width:40%;");
            builder.Hidden(f => f.Id);
            builder.Table(table =>
            {
                table.ColGroup(100, null);
                table.Tr(attr => builder.Field<Text>(f => f.Name).Build());
                table.Tr(attr => builder.Field<CheckBox>(f => f.Enabled).Build());
                table.Tr(attr => builder.Field<TextArea>(f => f.Note).Build());
            });
        });
        BuildRoleModules(builder.Builder);
        BuildRoleButtons(builder.Builder);
        BuildRoleColumns(builder.Builder);
    }

    protected override void BuildButtons(RenderTreeBuilder builder)
    {
        builder.Button(FormButton.Save, Callback(OnSave), !ReadOnly);
        base.BuildButtons(builder);
    }

    private void BuildRoleModules(RenderTreeBuilder builder)
    {
        builder.Div("role-module", attr =>
        {
            builder.Component<Tree<MenuInfo>>()
                   .Set(c => c.Data, data)
                   .Set(c => c.ReadOnly, ReadOnly)
                   .Set(c => c.ShowCheckBox, true)
                   .Set(c => c.Values, values)
                   .Set(c => c.OnItemClick, Callback<TreeItem<MenuInfo>>(v => curItem = v))
                   .Set(c => c.ValuesChanged, Callback<HashSet<MenuInfo>>(v => values = v))
                   .Build();
        });
    }

    private void BuildRoleButtons(RenderTreeBuilder builder)
    {
        builder.Div("role-button", attr =>
        {
            if (curItem != null)
            {
                var menu = curItem.Value;
                var items = new List<CodeInfo>();
                if (menu.Buttons != null && menu.Buttons.Count > 0)
                    items.AddRange(menu.Buttons.Select(b => new CodeInfo($"b_{menu.Id}_{b}", b)));
                if (menu.Actions != null && menu.Actions.Count > 0)
                    items.AddRange(menu.Actions.Select(b => new CodeInfo($"b_{menu.Id}_{b}", b)));
                var value = btnValues[menu.Id];
                builder.Component<CheckList>()
                       .Set(c => c.IsInput, true)
                       .Set(c => c.Items, items.ToArray())
                       .Set(c => c.Value, value)
                       .Set(c => c.ValueChanged, OnButtonValueChanged)
                       .Build();
            }
        });
    }

    private void BuildRoleColumns(RenderTreeBuilder builder)
    {
        builder.Div("role-column", attr =>
        {
            if (curItem != null)
            {
                var menu = curItem.Value;
                var items = new List<CodeInfo>();
                if (menu.Columns != null && menu.Columns.Count > 0)
                    items.AddRange(menu.Columns.Select(b => new CodeInfo($"c_{menu.Id}_{b.Id}", b.Name)));
                var value = colValues[menu.Id];
                builder.Component<CheckList>()
                       .Set(c => c.IsInput, true)
                       .Set(c => c.Items, items.ToArray())
                       .Set(c => c.Value, value)
                       .Set(c => c.ValueChanged, OnColumnValueChanged)
                       .Build();
            }
        });
    }

    private void OnButtonValueChanged(string value) => btnValues[curItem.Value.Id] = value;
    private void OnColumnValueChanged(string value) => colValues[curItem.Value.Id] = value;

    private void OnSave()
    {
        var menuIds = values.Select(v => v.Id).ToList();
        var buttonIds = btnValues.Values.Where(v => !string.IsNullOrWhiteSpace(v)).ToList();
        foreach (var item in buttonIds)
        {
            menuIds.AddRange(item.Split(','));
        }
        var columnIds = colValues.Values.Where(v => !string.IsNullOrWhiteSpace(v)).ToList();
        foreach (var item in columnIds)
        {
            menuIds.AddRange(item.Split(','));
        }
        SubmitAsync(data =>
        {
            var info = new RoleFormInfo { Model = data, MenuIds = menuIds };
            return Platform.Role.SaveRoleAsync(info);
        });
    }
}