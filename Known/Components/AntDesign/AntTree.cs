﻿using AntDesign;

namespace Known.Components;

/// <summary>
/// 扩展Ant树组件类。
/// </summary>
public class AntTree : Tree<MenuInfo>
{
    /// <summary>
    /// 取得或设置UI上下文对象级联值实例。
    /// </summary>
    [CascadingParameter] public UIContext Context { get; set; }

    /// <summary>
    /// 取得或设置树组件模型对象实例。
    /// </summary>
	[Parameter] public TreeModel Model { get; set; }

    /// <inheritdoc />
	protected override void OnInitialized()
    {
        base.OnInitialized();
        BlockNode = true;
        ShowIcon = true;
        CheckOnClickNode = false;
        //DisabledExpression = x => !x.DataItem.Enabled || Model.IsView;
        KeyExpression = x => x.DataItem.Id;
        TitleExpression = x => Context?.Language[x.DataItem.Name];
        IconExpression = x => x.DataItem.Icon;
        ChildrenExpression = x => x.DataItem.Children;
        IsLeafExpression = x => x.DataItem.Children?.Count == 0;
        TitleIconTemplate = this.BuildTree<TreeNode<MenuInfo>>((b, t) => b.Icon(t.Icon));
        OnClick = this.Callback<TreeEventArgs<MenuInfo>>(OnTreeClick);
        OnCheck = this.Callback<TreeEventArgs<MenuInfo>>(OnTreeCheck);
        if (Model != null)
            Model.OnRefresh = RefreshAsync;
    }

    /// <inheritdoc />
    protected override async Task OnParametersSetAsync()
    {
        if (Model == null)
            return;

        await base.OnParametersSetAsync();
        DataSource = Model.Data;
        Checkable = Model.Checkable;
        //DefaultExpandParent = Model.ExpandRoot;
        if (Model.DefaultExpandedKeys != null && Model.DefaultExpandedKeys.Length > 0)
            DefaultExpandedKeys = Model.DefaultExpandedKeys;
        else if (Model.ExpandRoot && Model.Data != null && Model.Data.Count > 0)
            DefaultExpandedKeys = [.. Model.Data.Select(d => d.Id)];
        DefaultSelectedKeys = Model.SelectedKeys;
        DefaultCheckedKeys = Model.CheckedKeys;
        DisableCheckKeys = Model.DisableCheckKeys;
    }

    private async Task RefreshAsync()
    {
        if (Model.OnModelChanged != null)
            Model = await Model.OnModelChanged.Invoke();
        DataSource = Model.Data;
        if (Model.DefaultExpandedKeys != null && Model.DefaultExpandedKeys.Length > 0)
            DefaultExpandedKeys = Model.DefaultExpandedKeys;
        else if (Model.ExpandRoot && Model.Data != null && Model.Data.Count > 0)
            DefaultExpandedKeys = [.. Model.Data.Select(d => d.Id)];
        DefaultSelectedKeys = Model.SelectedKeys;
        DefaultCheckedKeys = Model.CheckedKeys;
        DisableCheckKeys = Model.DisableCheckKeys;
        StateHasChanged();
    }

    private void OnTreeClick(TreeEventArgs<MenuInfo> e)
    {
        var item = e.Node.DataItem;
        item.Checked = e.Node.Checked;
        Model.OnNodeClick?.Invoke(item);
    }

    private void OnTreeCheck(TreeEventArgs<MenuInfo> e)
    {
        var item = e.Node.DataItem;
        item.Checked = e.Node.Checked;
        Model.OnNodeCheck?.Invoke(item);
    }
}