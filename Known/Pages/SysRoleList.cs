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

    public async void New()
    {
        var model = await roleService.GetRoleAsync("");
        Table.NewForm(roleService.SaveRoleAsync, model);
    }

    public async void Edit(SysRole row)
    {
        var model = await roleService.GetRoleAsync(row.Id);
        Table.EditForm(roleService.SaveRoleAsync, model);
    }

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
        var modules = Model.Data.Modules;
        if (modules == null || modules.Count == 0)
        {
            var service = await CreateServiceAsync<IRoleService>();
            var model = await service.GetRoleAsync(Model.Data.Id);
            modules = model.Modules;
        }

        var data = modules?.ToMenuItems(false);
        tree = new TreeModel
        {
            Checkable = true,
            IsView = Model.IsView,
            Data = data ?? [],
            DefaultCheckedKeys = [.. Model.Data.MenuIds],
            OnNodeClick = OnTreeClick,
            OnNodeCheck = OnTreeCheck
        };

        btnModel.ValueChanged = this.Callback<string[]>(OnButtonChanged);
        colModel.ValueChanged = this.Callback<string[]>(OnColumnChanged);
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