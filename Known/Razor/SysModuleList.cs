using Known.Entities;
using Known.Extensions;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Razor;

class SysModuleList : BasePage<SysModule>
{
	private MenuItem current;
    private int total;

	protected override async Task OnInitPageAsync()
	{
		await base.OnInitPageAsync();
        
        Page.FormTitle = row => $"{Name} - {row.ParentName}";
        //Page.Form.Width = 1000;
        //Page.Form.NoFooter = true;
        Page.Table.ShowPager = false;

        Page.Tree = new TreeModel
        {
            ExpandParent = true,
            OnNodeClick = OnNodeClick,
            OnRefresh = OnTreeRefresh
        };
        await OnTreeRefresh();
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (!Config.App.IsDevelopment)
        {
            UI.BuildResult(builder, "403", "非开发环境，不可访问该页面！");
            return;
        }
        base.BuildRenderTree(builder);
    }

    protected override Task<PagingResult<SysModule>> OnQueryAsync(PagingCriteria criteria)
	{
        var data = current?.Children?.Select(c => (SysModule)c.Data).ToList();
        total = data?.Count ?? 0;
        var result = new PagingResult<SysModule> { PageData = data, TotalCount = total };
		return Task.FromResult(result);
	}

    public void New()
    {
        if (current == null)
        {
            UI.Error("请先选择上级模块！");
            return;
        }

        Page.NewForm(Platform.Module.SaveModuleAsync, new SysModule { ParentId = current?.Id, ParentName = current?.Name, Sort = total + 1 });
    }

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
		current = item;
		await Page.Table.RefreshAsync();
	}

	private async Task OnTreeRefresh()
	{
        var datas = await Platform.Module.GetModulesAsync();
        var root = new MenuItem("0", Config.App.Name, "desktop");
        root.Children.AddRange(datas.ToMenuItems(ref current));
        if (current == null || current.Id == "0")
            current = root;
        Page.Tree.Data = [root];
        Page.Tree.SelectedKeys = [current?.Id];
    }
}

//public class SysModuleForm : BaseForm<SysModule>
//{
//    //protected override async Task OnInitFormAsync()
//    //{
//    //    await base.OnInitFormAsync();
//    //    Model.Data = await Platform.Role.GetRoleAsync(Model.Data.Id);
//    //}
//}