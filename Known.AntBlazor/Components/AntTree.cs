using AntDesign;
using Known.Blazor;
using Microsoft.AspNetCore.Components;

namespace Known.AntBlazor.Components;

public class AntTree : Tree<Known.MenuItem>
{
	[Parameter] public TreeModel Model { get; set; }

	protected override void OnInitialized()
	{
		ShowIcon = true;
		Checkable = Model.Checkable;
		DefaultExpandParent = Model.ExpandParent;
		DefaultSelectedKeys = Model.SelectedKeys;
		DefaultCheckedKeys = Model.DefaultCheckedKeys;
		DisabledExpression = x => !x.DataItem.Enabled || Model.IsView;
		KeyExpression = x => x.DataItem.Id;
		TitleExpression = x => x.DataItem.Name;
		IconExpression = x => x.DataItem.Icon;
		ChildrenExpression = x => x.DataItem.Children;
		IsLeafExpression = x => x.DataItem.Children?.Count == 0;
		DataSource = Model.Data;
		OnClick = Callback<TreeEventArgs<Known.MenuItem>>(OnTreeClick);
		OnCheck = Callback<TreeEventArgs<Known.MenuItem>>(OnTreeCheck);
		base.OnInitialized();
	}

	private void OnTreeClick(TreeEventArgs<Known.MenuItem> e)
	{
		var item = e.Node.DataItem;
        item.Checked = e.Node.Checked;
        Model.OnNodeClick?.Invoke(item);
	}

	private void OnTreeCheck(TreeEventArgs<Known.MenuItem> e)
	{
		var item = e.Node.DataItem;
		item.Checked = e.Node.Checked;
        Model.OnNodeCheck?.Invoke(item);
    }

	private EventCallback<T> Callback<T>(Action<T> callback) => EventCallback.Factory.Create(this, callback);
}