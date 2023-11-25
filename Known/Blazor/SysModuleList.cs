using Known.Entities;
using Known.Extensions;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Blazor;

class SysModuleList : BasePage<SysModule>
{
    private List<SysModule> modules;
    private MenuItem current;
    private int total;

    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();

        Page.FormTitle = row => $"{Name} - {row.ParentName}";
        Page.Form.Width = 1000;
        Page.Form.NoFooter = true;
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
        var result = new PagingResult<SysModule>(data);
        return Task.FromResult(result);
    }

    [Action]
    public void New()
    {
        if (current == null)
        {
            UI.Error("请先选择上级模块！");
            return;
        }

        Page.NewForm(Platform.Module.SaveModuleAsync, new SysModule { ParentId = current?.Id, ParentName = current?.Name, Sort = total + 1 });
    }

    [Action] public void Edit(SysModule row) => Page.EditForm(Platform.Module.SaveModuleAsync, row);
    [Action] public void Delete(SysModule row) => Page.Delete(Platform.Module.DeleteModulesAsync, row);
    [Action] public void DeleteM() => Page.DeleteM(Platform.Module.DeleteModulesAsync);

    [Action] public void Copy() => Page.Table.SelectRows(OnCopy);
    [Action] public void Move() => Page.Table.SelectRows(OnMove);
    [Action] public void MoveUp(SysModule row) => OnMove(row, true);
    [Action] public void MoveDown(SysModule row) => OnMove(row, false);

    private void OnCopy(List<SysModule> rows)
    {
        OpenTreeModal("复制到", node =>
        {
            rows.ForEach(m => m.ParentId = node.Id);
            return Platform.Module.CopyModulesAsync(rows);
        });
    }

    private void OnMove(List<SysModule> rows)
    {
        OpenTreeModal("移动到", node =>
        {
            rows.ForEach(m => m.ParentId = node.Id);
            return Platform.Module.MoveModulesAsync(rows);
        });
    }

    private async void OnMove(SysModule row, bool isMoveUp)
    {
        row.IsMoveUp = isMoveUp;
        var result = await Platform.Module.MoveModuleAsync(row);
        UI.Result(result, async () => await Page.RefreshAsync());
    }

    private async void OnNodeClick(MenuItem item)
    {
        current = item;
        await Page.Table.RefreshAsync();
    }

    private async Task OnTreeRefresh()
    {
        modules = await Platform.Module.GetModulesAsync();
        Page.Tree.Data = modules.ToMenuItems(ref current);
        if (current == null || current.Id == "0")
            current = Page.Tree.Data[0];
        Page.Tree.SelectedKeys = [current?.Id];
    }

    private void OpenTreeModal(string title, Func<SysModule, Task<Result>> action)
    {
        SysModule node = null;
        var model = new ModalOption
        {
            Title = title,
            Content = builder =>
            {
                UI.BuildTree(builder, new TreeModel
                {
                    ExpandParent = true,
                    Data = modules.ToMenuItems(),
                    OnNodeClick = n => node = n.Data as SysModule
                });
            }
        };
        model.OnOk = async () =>
        {
            if (node == null)
            {
                UI.Error("请选择模块！");
                return;
            }

            var result = await action?.Invoke(node);
            UI.Result(result, async () =>
            {
                await model.OnClose?.Invoke();
                await Page.RefreshAsync();
            });
        };
        UI.ShowModal(model);
    }
}