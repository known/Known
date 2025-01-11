namespace Known.Pages;

/// <summary>
/// 角色管理模块页面组件类。
/// </summary>
[StreamRendering]
[Route("/sys/roles")]
[Menu(Constants.System, "角色管理", "team", 2)]
public class SysRoleList : BaseTablePage<SysRole>
{
    /// <inheritdoc />
    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();

        Table.FormType = typeof(RoleForm);
        Table.OnQuery = Admin.QueryRolesAsync;
        Table.RowKey = r => r.Id;
    }

    /// <summary>
    /// 弹出新增表单对话框。
    /// </summary>
    [Action] public void New() => Table.NewForm(Admin.SaveRoleAsync, new SysRole());

    /// <summary>
    /// 弹出编辑表单对话框。
    /// </summary>
    /// <param name="row">表格行绑定的对象。</param>
    [Action] public void Edit(SysRole row) => Table.EditForm(Admin.SaveRoleAsync, row);

    /// <summary>
    /// 删除一条数据。
    /// </summary>
    /// <param name="row">表格行绑定的对象。</param>
    [Action] public void Delete(SysRole row) => Table.Delete(Admin.DeleteRolesAsync, row);

    /// <summary>
    /// 批量删除多条数据。
    /// </summary>
    [Action] public void DeleteM() => Table.DeleteM(Admin.DeleteRolesAsync);
}

class RoleForm : BaseForm<SysRole>
{
    private TreeModel tree;
    private MenuInfo current;
    private readonly InputModel<string[]> btnModel = new();
    private readonly InputModel<string[]> colModel = new();
    private bool ChkDisabled => Model.IsView || current == null || !current.Checked;

    protected override async Task OnInitFormAsync()
    {
        await base.OnInitFormAsync();
        Model.SmallLabel = true;

        tree = new TreeModel
        {
            Checkable = true,
            IsView = Model.IsView,
            OnNodeClick = OnTreeClickAsync,
            OnNodeCheck = OnTreeCheckAsync,
            OnModelChanged = OnTreeModelChangedAsync
        };

        btnModel.ValueChanged = this.Callback<string[]>(OnButtonChanged);
        colModel.ValueChanged = this.Callback<string[]>(OnColumnChanged);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
            await tree.RefreshAsync();
    }

    protected override void BuildForm(RenderTreeBuilder builder)
    {
        builder.Div("kui-role", () =>
        {
            builder.Div("kui-role-form", () => base.BuildForm(builder));
            builder.Div("kui-role-module", () =>
            {
                builder.Div("", Language["Title.Module"]);
                builder.Tree(tree);
            });
            builder.Div("kui-role-button", () =>
            {
                builder.Div("", Language["Title.Button"]);
                builder.CheckList(btnModel);
            });
            builder.Div("kui-role-column", () =>
            {
                builder.Div("", Language["Title.Column"]);
                builder.CheckList(colModel);
            });
        });
    }

    private Task OnTreeClickAsync(MenuInfo item)
    {
        SelectNode(item);
        return StateChangedAsync();
    }

    private Task OnTreeCheckAsync(MenuInfo item)
    {
        CheckNode(item, item.Checked);
        return OnTreeClickAsync(item);
    }

    private async Task<TreeModel> OnTreeModelChangedAsync()
    {
        var model = await Admin.GetRoleAsync(Model.Data.Id);
        Model.Data.MenuIds = model.MenuIds;
        if (Model.IsView)
            tree.DisableCheckKeys = model.Modules.Select(m => m.Id).ToArray();
        tree.Data = model.Modules?.ToMenuItems(false);
        if (Model.Data.MenuIds != null)
            tree.CheckedKeys = [.. Model.Data.MenuIds];
        return tree;
    }

    private void OnButtonChanged(string[] items)
    {
        SetMenuData($"b_{current.Id}", items);
        btnModel.Value = [.. Model.Data.MenuIds];
    }

    private void OnColumnChanged(string[] items)
    {
        SetMenuData($"c_{current.Id}", items);
        colModel.Value = [.. Model.Data.MenuIds];
    }

    private void SelectNode(MenuInfo item)
    {
        current = item;

        btnModel.Disabled = ChkDisabled;
        btnModel.Codes = current.GetAllActions();
        btnModel.Value = [.. Model.Data.MenuIds];

        colModel.Disabled = ChkDisabled;
        colModel.Codes = current.GetAllColumns();
        colModel.Value = [.. Model.Data.MenuIds];
    }

    private void CheckNode(MenuInfo item, bool isChecked)
    {
        if (item.Children.Count > 0)
        {
            foreach (var child in item.Children)
            {
                CheckNode(child, isChecked);
            }
        }
        else
        {
            CheckItem(item, isChecked);
        }
    }

    private void CheckItem(MenuInfo item, bool isChecked)
    {
        var btnItems = isChecked ? item.GetAllActions().Select(o => o.Code).ToArray() : null;
        var colItems = isChecked ? item.GetAllColumns().Select(o => o.Code).ToArray() : null;
        SetMenuData($"b_{item.Id}", btnItems);
        SetMenuData($"c_{item.Id}", colItems);

        Model.Data.MenuIds.Remove(item.Id);
        if (isChecked)
            Model.Data.MenuIds.Add(item.Id);
    }

    private void SetMenuData(string prefix, string[] items)
    {
        Model.Data.MenuIds.RemoveAll(m => m.StartsWith(prefix));
        if (items != null && items.Length > 0)
            Model.Data.MenuIds.AddRange(items);
    }
}