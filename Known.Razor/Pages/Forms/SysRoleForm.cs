namespace Known.Razor.Pages.Forms;

[Dialog(1000, 500)]
class SysRoleForm : BaseForm<SysRole>
{
    class CheckInfo
    {
        public bool IsChecked { get; set; }
        public List<CodeInfo> Items { get; set; }
        public string Value { get; set; }

        private bool isAll;
        public bool IsAll
        {
            get { return isAll; }
            set
            {
                isAll = value;
                Value = isAll
                      ? string.Join(",", Items.Select(c => c.Code))
                      : string.Empty;
            }
        }

        internal void SetIsAll() => isAll = IsCheckAll(Items, Value);

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
        builder.Div("form", attr =>
        {
            builder.Div("form-body", attr =>
            {
                builder.Hidden(f => f.Id);
                builder.Table(table =>
                {
                    table.ColGroup(100, null);
                    table.Tr(attr => table.Field<Text>(f => f.Name).Build());
                    table.Tr(attr => table.Field<CheckBox>(f => f.Enabled).Set(f => f.Switch, true).Build());
                    table.Tr(attr => table.Field<TextArea>(f => f.Note).Build());
                });
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
                   .Set(c => c.OnCheckClick, Callback<TreeItem<MenuInfo>>(OnMenuItemCheck))
                   .Set(c => c.OnItemClick, Callback<TreeItem<MenuInfo>>(OnMenuItemClick))
                   .Set(c => c.ValuesChanged, Callback<HashSet<MenuInfo>>(v => values = v))
                   .Build();
        });
    }

    private void OnMenuItemCheck(TreeItem<MenuInfo> item) => SetCurItem(item, true);
    private void OnMenuItemClick(TreeItem<MenuInfo> item) => SetCurItem(item);

    private void SetCurItem(TreeItem<MenuInfo> item, bool isCheck = false)
    {
        curItem = item;
        curButton = btnValues[item.Value.Id] ?? new CheckInfo();
        curButton.IsChecked = item.IsChecked;
        curColumn = colValues[item.Value.Id] ?? new CheckInfo();
        curColumn.IsChecked = item.IsChecked;

        if (isCheck)
        {
            curButton.IsAll = item.IsChecked;
            curColumn.IsAll = item.IsChecked;
        }
    }

    private void BuildRoleButtons(RenderTreeBuilder builder)
    {
        builder.Component<RoleCheckList>()
               .Set(c => c.Style, "role-button")
               .Set(c => c.ReadOnly, ReadOnly)
               .Set(c => c.Title, "按钮")
               .Set(c => c.Info, curButton)
               .Set(c => c.OnChanged, OnButtonChanged)
               .Build();
    }

    private void OnButtonChanged(CheckInfo info) => btnValues[curItem.Value.Id] = info;

    private void BuildRoleColumns(RenderTreeBuilder builder)
    {
        builder.Component<RoleCheckList>()
               .Set(c => c.Style, "role-column")
               .Set(c => c.ReadOnly, ReadOnly)
               .Set(c => c.Title, "栏位")
               .Set(c => c.Info, curColumn)
               .Set(c => c.OnChanged, OnColumnChanged)
               .Build();
    }

    private void OnColumnChanged(CheckInfo info) => colValues[curItem.Value.Id] = info;

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

    class RoleCheckList : BaseComponent
    {
        [Parameter] public string Style { get; set; }
        [Parameter] public string Title { get; set; }
        [Parameter] public CheckInfo Info { get; set; }
        [Parameter] public Action<CheckInfo> OnChanged { get; set; }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.Div(Style, attr =>
            {
                if (Info == null)
                {
                    builder.Div("title", Title);
                }
                else
                {
                    BuildTitle(builder);
                    builder.Component<CheckList>()
                           .Set(c => c.ReadOnly, ReadOnly)
                           .Set(c => c.IsInput, true)
                           .Set(c => c.Enabled, Info.IsChecked)
                           .Set(c => c.Items, Info.Items.ToArray())
                           .Set(c => c.Value, Info.Value)
                           .Set(c => c.ValueChanged, OnValueChanged)
                           .Build();
                }
            });
        }

        private void BuildTitle(RenderTreeBuilder builder)
        {
            builder.Div("title", attr =>
            {
                builder.Span(Title);
                builder.Check(attr =>
                {
                    attr.Title("全选/取消").Disabled(!Info.IsChecked || ReadOnly).Checked(Info.IsAll)
                        .OnClick(Callback(() => Info.IsAll = !Info.IsAll));
                });
            });
        }

        private void OnValueChanged(string value)
        {
            Info.Value = value;
            OnChanged?.Invoke(Info);
        }
    }
}