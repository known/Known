using Known.Entities;
using Known.Extensions;

namespace Known.Razor;

class SysModuleList : BasePage<SysModule>
{
    private List<SysModule> datas;
	private MenuItem parent;

	protected override async Task OnInitPageAsync()
	{
        datas = await Platform.Module.GetModulesAsync();
		await base.OnInitPageAsync();
        Page.Tree = new TreeModel
		{
			Data = datas.ToMenuItems(),
			OnNodeClick = OnNodeClick,
            OnRefresh = OnTreeRefresh
		};
	}

	protected override Task<PagingResult<SysModule>> OnQueryAsync(PagingCriteria criteria)
	{
        var items = parent == null ? Page.Tree.Data : parent?.Children;
        var data = items.Select(c => (SysModule)c.Data).ToList();
		var result = new PagingResult<SysModule> { PageData = data, TotalCount = data?.Count ?? 0 };
		return Task.FromResult(result);
	}

	public void New() => Page.NewForm(Platform.Module.SaveModuleAsync, new SysModule());
    public void Edit(SysModule row) => Page.EditForm(Platform.Module.SaveModuleAsync, row);
    public void Delete(SysModule row) => Page.Delete(Platform.Module.DeleteModulesAsync, row);
    public void DeleteM() => Page.DeleteM(Platform.Module.DeleteModulesAsync);

    public void Copy() => Page.Table.SelectRows(OnCopy);
    public void Move() => Page.Table.SelectRows(OnMove);
    //public void MoveUp(SysModule row) => OnMove(row, true);
    //public void MoveDown(SysModule row) => OnMove(row, false);

    private void OnCopy(List<SysModule> models)
    {
        //SysModule node = null;
        //UI.Prompt("复制到", builder =>
        //{
        //    builder.Component<KTree<string>>()
        //           .Set(c => c.Data, data)
        //           .Set(c => c.OnItemClick, Callback<KTreeItem<string>>(n => node = n))
        //           .Build();
        //}, async model =>
        //{
        //    models.ForEach(m => m.ParentId = node.Id);
        //    var result = await Platform.Module.CopyModulesAsync(models);
        //    UI.Result(result, () =>
        //    {
        //        StateChanged();
        //    });
        //});
    }

    private void OnMove(List<SysModule> models)
    {
        //SysModule node = null;
        //UI.Prompt("移动到", builder =>
        //{
        //    builder.Component<KTree<string>>()
        //           .Set(c => c.Data, data)
        //           .Set(c => c.OnItemClick, Callback<KTreeItem<string>>(n => node = n))
        //           .Build();
        //}, async model =>
        //{
        //    models.ForEach(m => m.ParentId = node.Id);
        //    var result = await Platform.Module.MoveModulesAsync(models);
        //    UI.Result(result, () =>
        //    {
        //        StateChanged();
        //    });
        //});
    }

	private async void OnNodeClick(MenuItem item)
	{
		parent = item;
		await Page.Table.RefreshAsync();
	}

	private async Task OnTreeRefresh()
	{
		datas = await Platform.Module.GetModulesAsync();
        Page.Tree.Data = datas.ToMenuItems();
	}
}