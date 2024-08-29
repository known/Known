namespace Known.Pages;

[StreamRendering]
[Route("/sys/roles")]
public class SysRoleList : BaseTablePage<SysRole>
{
    private IRoleService Service;

    protected override async Task OnPageInitAsync()
    {
        await base.OnPageInitAsync();
        Service = await CreateServiceAsync<IRoleService>();

        Table.FormType = typeof(RoleForm);
        Table.OnQuery = Service.QueryRolesAsync;
        Table.RowKey = r => r.Id;
    }

    public void New() => Table.NewForm(Service.SaveRoleAsync, new SysRole());
    public void Edit(SysRole row) => Table.EditForm(Service.SaveRoleAsync, row);
    public void Delete(SysRole row) => Table.Delete(Service.DeleteRolesAsync, row);
    public void DeleteM() => Table.DeleteM(Service.DeleteRolesAsync);
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
        tree = new TreeModel
        {
            Checkable = true,
            IsView = Model.IsView,
            OnNodeClick = OnTreeClick,
            OnNodeCheck = OnTreeCheck,
            OnModelChanged = OnTreeModelChanged
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
                UI.BuildTree(builder, tree);
            });
            builder.Div("kui-role-button", () =>
            {
                builder.Div("", Language["Title.Button"]);
                UI.BuildCheckList(builder, btnModel);
            });
            builder.Div("kui-role-column", () =>
            {
                builder.Div("", Language["Title.Column"]);
                UI.BuildCheckList(builder, colModel);
            });
        });
    }

    private void OnTreeClick(MenuInfo item)
    {
        SelectNode(item);
        StateChanged();
    }

    private void OnTreeCheck(MenuInfo item)
    {
        CheckNode(item, item.Checked);
        OnTreeClick(item);
    }

    private async Task<TreeModel> OnTreeModelChanged()
    {
        var service = await CreateServiceAsync<IRoleService>();
        var model = await service.GetRoleAsync(Model.Data.Id);
        Model.Data.MenuIds = model.MenuIds;
        if (Model.IsView)
            tree.DisableCheckKeys = model.Modules.Select(m => m.Id).ToArray();
        tree.Data = model.Modules?.ToMenuItems(false);
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