namespace Known.AntBlazor.Components;

public class AntTree : Tree<MenuInfo>
{
	[Parameter] public TreeModel Model { get; set; }

	protected override void OnInitialized()
	{
        if (Model == null)
            return;

        Model.OnRefresh = RefreshAsync;
        ShowIcon = true;
        CheckOnClickNode = false;
        DisabledExpression = x => !x.DataItem.Enabled || Model.IsView;
        KeyExpression = x => x.DataItem.Id;
        TitleExpression = x => x.DataItem.Name;
        IconExpression = x => x.DataItem.Icon;
        ChildrenExpression = x => x.DataItem.Children;
        IsLeafExpression = x => x.DataItem.Children?.Count == 0;
        OnClick = this.Callback<TreeEventArgs<MenuInfo>>(OnTreeClick);
		OnCheck = this.Callback<TreeEventArgs<MenuInfo>>(OnTreeCheck);
        base.OnInitialized();
	}

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
        DefaultCheckedKeys = Model.DefaultCheckedKeys;
    }

    private async Task RefreshAsync()
    {
        Model = await Model.OnModelChanged?.Invoke();
        DataSource = Model.Data;
        Console.WriteLine($"DC={Model.Data.Count}");
        DefaultSelectedKeys = Model.SelectedKeys;
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