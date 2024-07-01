namespace Known.Pages;

//[Authorize]
[StreamRendering]
[Route("/sys/roles")]
public class SysRoleList : BaseTablePage<SysRole>
{
    private IRoleService roleService;

    protected override async Task OnPageInitAsync()
    {
        await base.OnPageInitAsync();
        roleService = await CreateServiceAsync<IRoleService>();

        Table.FormType = typeof(RoleForm);
        Table.OnQuery = roleService.QueryRolesAsync;
        Table.RowKey = r => r.Id;
    }

    public void New() => Table.NewForm(roleService.SaveRoleAsync, new SysRole());
    public void Edit(SysRole row) => Table.EditForm(roleService.SaveRoleAsync, row);
    public void Delete(SysRole row) => Table.Delete(roleService.DeleteRolesAsync, row);
    public void DeleteM() => Table.DeleteM(roleService.DeleteRolesAsync);
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
        SelectNode(item);

        var btnItems = item.Checked ? btnModel.Codes.Select(o => o.Code).ToArray() : null;
        var colItems = item.Checked ? colModel.Codes.Select(o => o.Code).ToArray() : null;
        OnButtonChanged(btnItems);
        OnColumnChanged(colItems);

        Model.Data.MenuIds.Remove(item.Id);
        if (item.Checked)
            Model.Data.MenuIds.Add(item.Id);
        StateChanged();
    }

    private async Task<TreeModel> OnTreeModelChanged()
    {
        var service = await CreateServiceAsync<IRoleService>();
        var model = await service.GetRoleAsync(Model.Data.Id);
        Model.Data.MenuIds = model.MenuIds;
        tree.Data = model.Modules?.ToMenuItems(false);
        tree.CheckedKeys = [.. Model.Data.MenuIds];
        return tree;
    }

    private void OnButtonChanged(string[] items)
    {
        Model.Data.MenuIds.RemoveAll(m => m.StartsWith($"b_{current.Id}"));
        if (items != null && items.Length > 0)
            Model.Data.MenuIds.AddRange(items);
        btnModel.Value = [.. Model.Data.MenuIds];
    }

    private void OnColumnChanged(string[] items)
    {
        Model.Data.MenuIds.RemoveAll(m => m.StartsWith($"c_{current.Id}"));
        if (items != null && items.Length > 0)
            Model.Data.MenuIds.AddRange(items);
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
}