using Known.Entities;
using Known.Extensions;

namespace Known.Razor;

class SysModuleList : BasePage<SysModule>
{
    private List<SysModule> datas;

    protected override async Task OnInitPageAsync()
	{
        datas = await Platform.Module.GetModulesAsync();
		await base.OnInitPageAsync();
		Tree = new TreeModel
		{
			Data = datas.ToMenuItems()
		};
	}

    public void New() => Table.ShowForm(Platform.Module.SaveModuleAsync, new SysModule());
    public void Edit(SysModule row) => Table.ShowForm(Platform.Module.SaveModuleAsync, row);
    public void Delete(SysModule row) => Table.Delete(Platform.Module.DeleteModulesAsync, row);
    public void DeleteM() => Table.DeleteM(Platform.Module.DeleteModulesAsync);

    public void Copy() => Table.SelectRows(OnCopy);
    public void Move() => Table.SelectRows(OnMove);
    //public void MoveUp(SysModule row) => OnMove(row, true);
    //public void MoveDown(SysModule row) => OnMove(row, false);

    private void OnCopy(List<SysModule> models)
    {
        SysModule node = null;
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
        SysModule node = null;
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
}