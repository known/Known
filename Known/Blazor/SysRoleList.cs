using Known.Entities;
using Known.Extensions;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Blazor;

class SysRoleList : BaseTablePage<SysRole>
{
    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();
        Table.Form.Width = 1000;
        Table.OnQuery = Platform.Role.QueryRolesAsync;
		Table.RowKey = r => r.Id;
    }

    [Action] public void New() => Table.NewForm(Platform.Role.SaveRoleAsync, new SysRole());
    [Action] public void Edit(SysRole row) => Table.EditForm(Platform.Role.SaveRoleAsync, row);
    [Action] public void Delete(SysRole row) => Table.Delete(Platform.Role.DeleteRolesAsync, row);
    [Action] public void DeleteM() => Table.DeleteM(Platform.Role.DeleteRolesAsync);
}

class SysRoleForm : BaseForm<SysRole>
{
    private TreeModel tree;
    private MenuItem current;
    private readonly InputModel<string[]> btnModel = new();
    private readonly InputModel<string[]> colModel = new();
    private bool ChkDisabled => Model.IsView || current == null || !current.Checked;

    protected override async Task OnInitFormAsync()
    {
        await base.OnInitFormAsync();
        Model.Data = await Platform.Role.GetRoleAsync(Model.Data.Id);
        tree = new TreeModel
        {
            Checkable = true,
            IsView = Model.IsView,
            Data = Model.Data.Menus,
            DefaultCheckedKeys = Model.Data.MenuIds.ToArray(),
            OnNodeClick = OnTreeClick,
            OnNodeCheck = OnTreeCheck
        };

        btnModel.ValueChanged = this.Callback<string[]>(OnButtonChanged);
        colModel.ValueChanged = this.Callback<string[]>(OnColumnChanged);
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div("kui-role", () =>
        {
            builder.Div("kui-role-form", () => UI.BuildForm(builder, Model));
            builder.Div("kui-role-module", () =>
            {
                builder.Div("", Context.Language["Title.Module"]);
                UI.BuildTree(builder, tree);
            });
            builder.Div("kui-role-button", () =>
            {
                builder.Div("", Context.Language["Title.Button"]);
                UI.BuildCheckList(builder, btnModel);
            });
            builder.Div("kui-role-column", () =>
            {
                builder.Div("", Context.Language["Title.Column"]);
                UI.BuildCheckList(builder, colModel);
            });
        });
    }

    private void OnTreeClick(MenuItem item)
    {
        SelectNode(item);
        StateChanged();
    }

    private void OnTreeCheck(MenuItem item)
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
        btnModel.Value = Model.Data.MenuIds.ToArray();
    }

    private void OnColumnChanged(string[] items)
    {
        Model.Data.MenuIds.RemoveAll(m => m.StartsWith($"c_{current.Id}"));
        if (items != null && items.Length > 0)
            Model.Data.MenuIds.AddRange(items);
        colModel.Value = Model.Data.MenuIds.ToArray();
    }

    private void SelectNode(MenuItem item)
    {
        current = item;

        btnModel.Disabled = ChkDisabled;
        btnModel.Codes = current.GetAllActions();
        btnModel.Value = Model.Data.MenuIds.ToArray();

        colModel.Disabled = ChkDisabled;
        colModel.Codes = current.GetAllColumns();
        colModel.Value = Model.Data.MenuIds.ToArray();
    }
}