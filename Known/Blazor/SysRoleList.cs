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
    private readonly ListOption<string[]> btnOption = new();
    private readonly ListOption<string[]> colOption = new();
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

        btnOption.ValueChanged = this.Callback<string[]>(OnButtonChanged);
        colOption.ValueChanged = this.Callback<string[]>(OnColumnChanged);
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div("sys-role", () =>
        {
            builder.Div("sys-role-form", () => UI.BuildForm(builder, Model));
            builder.Div("sys-role-module", () =>
            {
                builder.Div("", "模块");
                UI.BuildTree(builder, tree);
            });
            builder.Div("sys-role-button", () =>
            {
                builder.Div("", "按钮");
                UI.BuildCheckList(builder, btnOption);
            });
            builder.Div("sys-role-column", () =>
            {
                builder.Div("", "栏位");
                UI.BuildCheckList(builder, colOption);
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

        var btnItems = item.Checked ? btnOption.Codes.Select(o => o.Code).ToArray() : null;
        var colItems = item.Checked ? colOption.Codes.Select(o => o.Code).ToArray() : null;
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
        btnOption.Value = Model.Data.MenuIds.ToArray();
    }

    private void OnColumnChanged(string[] items)
    {
        Model.Data.MenuIds.RemoveAll(m => m.StartsWith($"c_{current.Id}"));
        if (items != null && items.Length > 0)
            Model.Data.MenuIds.AddRange(items);
        colOption.Value = Model.Data.MenuIds.ToArray();
    }

    private void SelectNode(MenuItem item)
    {
        current = item;

        btnOption.Disabled = ChkDisabled;
        btnOption.Codes = current.GetAllActions();
        btnOption.Value = Model.Data.MenuIds.ToArray();

        colOption.Disabled = ChkDisabled;
        colOption.Codes = current.GetAllColumns();
        colOption.Value = Model.Data.MenuIds.ToArray();
    }
}