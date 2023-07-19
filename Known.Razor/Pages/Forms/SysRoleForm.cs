namespace Known.Razor.Pages.Forms;

[Dialog(1000, 500)]
class SysRoleForm : BaseForm<SysRole>
{
    class CheckInfo
    {
        public bool IsAll { get; set; }
        public List<CodeInfo> Items { get; set; }
        public string Value { get; set; }

        internal void SetIsAll()
        {
            IsAll = IsCheckAll(Items, Value);
        }

        internal static CheckInfo LoadButton(MenuInfo menu, List<string> menuIds)
        {
            var info = new CheckInfo
            {
                Items = menu.GetButtonCodes(),
                Value = string.Join(",", menuIds.Where(m => m.StartsWith($"b_{menu.Id}_")))
            };
            info.SetIsAll();
            return info;
        }

        internal static CheckInfo LoadColumn(MenuInfo menu, List<string> menuIds)
        {
            var info = new CheckInfo
            {
                Items = menu.GetColumnCodes(),
                Value = string.Join(",", menuIds.Where(m => m.StartsWith($"c_{menu.Id}_")))
            };
            info.SetIsAll();
            return info;
        }

        private static bool IsCheckAll(List<CodeInfo> codes, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            var values = value.Split(',');
            return values.Length == codes.Count;
        }
    }

    private readonly Dictionary<string, CheckInfo> btnValues = new();
    private readonly Dictionary<string, CheckInfo> colValues = new();
    private HashSet<MenuInfo> values = new();
    private RoleFormInfo info;
    private List<TreeItem<MenuInfo>> data;
    private TreeItem<MenuInfo> curItem;
    private CheckList chkButton;
    private CheckList chkColumn;
    private CheckInfo curButton;
    private CheckInfo curColumn;

    public SysRoleForm()
    {
        Style = "role";
    }

    protected override async Task InitFormAsync()
    {
        info = await Platform.Role.GetRoleAsync(TModel.Id);
        foreach (var item in info.Menus)
        {
            btnValues[item.Id] = CheckInfo.LoadButton(item, info.MenuIds);
            colValues[item.Id] = CheckInfo.LoadColumn(item, info.MenuIds);
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
                table.Tr(attr => table.Field<Text>(f => f.Name).Build());
                table.Tr(attr => table.Field<CheckBox>(f => f.Enabled).Set(f => f.Switch, true).Build());
                table.Tr(attr => table.Field<TextArea>(f => f.Note).Build());
            });
            builder.Div("form-button", attr =>
            {
                builder.Button(FormButton.Save, Callback(OnSave), !ReadOnly);
                builder.Button(FormButton.Close, Callback(OnCancel));
            });
        });
        BuildRoleModules(builder.Builder);
        BuildRoleButtons(builder.Builder);
        BuildRoleColumns(builder.Builder);
    }

    protected override void BuildButtons(RenderTreeBuilder builder) { }

    private void BuildRoleModules(RenderTreeBuilder builder)
    {
        builder.Div("role-module", attr =>
        {
            builder.Div("title", "模块");
            builder.Component<Tree<MenuInfo>>()
                   .Set(c => c.Data, data)
                   .Set(c => c.ReadOnly, ReadOnly)
                   .Set(c => c.ShowCheckBox, true)
                   .Set(c => c.Values, values)
                   .Set(c => c.OnItemClick, Callback<TreeItem<MenuInfo>>(OnMenuItemClick))
                   .Set(c => c.ValuesChanged, Callback<HashSet<MenuInfo>>(v => values = v))
                   .Build();
        });
    }

    private void BuildRoleButtons(RenderTreeBuilder builder)
    {
        builder.Div("role-button", attr =>
        {
            if (curItem == null)
            {
                builder.Div("title", "按钮");
            }
            else
            {
                var menu = curItem.Value;
                BuildButtonTitle(builder);
                builder.Component<CheckList>()
                       .Set(c => c.IsInput, true)
                       .Set(c => c.Items, curButton.Items.ToArray())
                       .Set(c => c.Value, curButton.Value)
                       .Set(c => c.ValueChanged, OnButtonValueChanged)
                       .Build(value => chkButton = value);
            }
        });
    }

    private void BuildRoleColumns(RenderTreeBuilder builder)
    {
        builder.Div("role-column", attr =>
        {
            if (curItem == null)
            {
                builder.Div("title", "栏位");
            }
            else
            {
                var menu = curItem.Value;
                BuildColumnTitle(builder);
                builder.Component<CheckList>()
                       .Set(c => c.IsInput, true)
                       .Set(c => c.Items, curColumn.Items.ToArray())
                       .Set(c => c.Value, curColumn.Value)
                       .Set(c => c.ValueChanged, OnColumnValueChanged)
                       .Build(value => chkColumn = value);
            }
        });
    }

    private void BuildButtonTitle(RenderTreeBuilder builder)
    {
        builder.Div("title", attr =>
        {
            builder.Span("按钮");
            builder.Check(attr => attr.Title("全选/取消").Checked(curButton.IsAll).OnClick(Callback(() =>
            {
                curButton.IsAll = !curButton.IsAll;
                if (curButton.IsAll)
                    chkButton.SetValue("");
                else
                    chkButton.SetValue("");
            })));
        });
    }

    private void BuildColumnTitle(RenderTreeBuilder builder)
    {
        builder.Div("title", attr =>
        {
            builder.Span("栏位");
            builder.Check(attr => attr.Title("全选/取消").Checked(curColumn.IsAll).OnClick(Callback(() =>
            {
                curColumn.IsAll = !curColumn.IsAll;
                if (curColumn.IsAll)
                    chkColumn.SetValue("");
                else
                    chkColumn.SetValue("");
            })));
        });
    }

    private void OnMenuItemClick(TreeItem<MenuInfo> item)
    {
        curItem = item;
        curButton = btnValues[item.Value.Id] ?? new CheckInfo();
        curColumn = colValues[item.Value.Id] ?? new CheckInfo();
    }

    private void OnButtonValueChanged(string value)
    {
        curButton.Value = value;
        btnValues[curItem.Value.Id] = curButton;
    }

    private void OnColumnValueChanged(string value)
    {
        curColumn.Value = value;
        colValues[curItem.Value.Id] = curColumn;
    }

    private void OnSave()
    {
        var menuIds = values.Select(v => v.Id).ToList();
        var buttons = btnValues.Values.Where(v => !string.IsNullOrWhiteSpace(v.Value)).ToList();
        foreach (var item in buttons)
        {
            menuIds.AddRange(item.Value.Split(','));
        }
        var columns = colValues.Values.Where(v => !string.IsNullOrWhiteSpace(v.Value)).ToList();
        foreach (var item in columns)
        {
            menuIds.AddRange(item.Value.Split(','));
        }
        SubmitAsync(data =>
        {
            var info = new RoleFormInfo { Model = data, MenuIds = menuIds };
            return Platform.Role.SaveRoleAsync(info);
        });
    }
}