using AntDesign;

namespace Known.Components;

/// <summary>
/// 扩展Ant树组件类。
/// </summary>
public class AntTree : Tree<MenuInfo>
{
    /// <summary>
    /// 取得或设置树组件模型对象实例。
    /// </summary>
	[Parameter] public TreeModel Model { get; set; }

    /// <summary>
    /// 初始化组件。
    /// </summary>
	protected override void OnInitialized()
	{
        base.OnInitialized();
        ShowIcon = true;
        CheckOnClickNode = false;
        //DisabledExpression = x => !x.DataItem.Enabled || Model.IsView;
        KeyExpression = x => x.DataItem.Id;
        TitleExpression = x => x.DataItem.Name;
        IconExpression = x => x.DataItem.Icon;
        ChildrenExpression = x => x.DataItem.Children;
        IsLeafExpression = x => x.DataItem.Children?.Count == 0;
        TitleIconTemplate = this.BuildTree<TreeNode<MenuInfo>>((b, t) => b.Icon(t.Icon));
        OnClick = this.Callback<TreeEventArgs<MenuInfo>>(OnTreeClick);
		OnCheck = this.Callback<TreeEventArgs<MenuInfo>>(OnTreeCheck);
        if (Model != null)
            Model.OnRefresh = RefreshAsync;
    }

    /// <summary>
    /// 异步设置组件参数。
    /// </summary>
    /// <returns></returns>
    protected override async Task OnParametersSetAsync()
    {
        if (Model == null)
            return;

        await base.OnParametersSetAsync();
        DataSource = Model.Data;
        Checkable = Model.Checkable;
        //DefaultExpandParent = Model.ExpandRoot;
        if (Model.ExpandRoot)
            DefaultExpandedKeys = [Model.Data?[0]?.Id];
        DefaultSelectedKeys = Model.SelectedKeys;
        DefaultCheckedKeys = Model.CheckedKeys;
        DisableCheckKeys = Model.DisableCheckKeys;
    }

    private async Task RefreshAsync()
    {
        Model = await Model.OnModelChanged?.Invoke();
        DataSource = Model.Data;
        if (Model.ExpandRoot)
            DefaultExpandedKeys = [Model.Data?[0]?.Id];
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