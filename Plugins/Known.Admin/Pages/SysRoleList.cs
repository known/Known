namespace Known.Pages;

[Route("/sys/roles")]
//[Menu(Constants.System, "角色管理", "team", 2)]
[PagePlugin("角色管理", "team", PagePluginType.Module, AdminLanguage.SystemManage, Sort = 5)]
public class SysRoleList : BaseTablePage<RoleInfo>
{
    private IRoleService Service;

    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();
        Service = await CreateServiceAsync<IRoleService>();

        Table.AdvSearch = UIConfig.IsAdvAdmin;
        Table.EnableFilter = UIConfig.IsAdvAdmin;
        Table.FormType = typeof(RoleForm);
        Table.Form = new FormInfo { Width = 1000, SmallLabel = true };
        Table.OnQuery = Service.QueryRolesAsync;
        Table.RowKey = r => r.Id;
    }

    [Action] public void New() => Table.NewForm(Service.SaveRoleAsync, new RoleInfo());
    [Action] public void Edit(RoleInfo row) => Table.EditForm(Service.SaveRoleAsync, row);
    [Action] public void Delete(RoleInfo row) => Table.Delete(Service.DeleteRolesAsync, row);
    [Action] public void DeleteM() => Table.DeleteM(Service.DeleteRolesAsync);
}

class RoleForm : BaseForm<RoleInfo>
{
    private IRoleService Service;
    private TreeModel tree;
    private MenuInfo current;
    private readonly InputModel<string[]> btnModel = new();
    private readonly InputModel<string[]> colModel = new();
    private bool ChkDisabled => Model.IsView || current == null || !current.Checked;

    protected override async Task OnInitFormAsync()
    {
        await base.OnInitFormAsync();
        Service = await CreateServiceAsync<IRoleService>();

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
                builder.Div("kui-bold", Language[AdminLanguage.Module]);
                builder.Tree(tree);
            });
            builder.Div("kui-role-button", () =>
            {
                builder.Div("kui-bold", Language[AdminLanguage.Button]);
                builder.CheckList(btnModel);
            });
            builder.Div("kui-role-column", () =>
            {
                builder.Div("kui-bold", Language[AdminLanguage.Column]);
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
        var model = await Service.GetRoleAsync(Model.Data.Id);
        Model.Data.MenuIds = model.MenuIds;
        if (Model.IsView)
            tree.DisableCheckKeys = [.. model.Modules.Select(m => m.Id)];
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
        btnModel.Codes = current.GetAllActions(Language);
        btnModel.Value = [.. Model.Data.MenuIds];

        colModel.Disabled = ChkDisabled;
        colModel.Codes = current.GetAllColumns(Language);
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
        var btnItems = isChecked ? item.GetAllActions(Language).Select(o => o.Code).ToArray() : null;
        var colItems = isChecked ? item.GetAllColumns(Language).Select(o => o.Code).ToArray() : null;
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