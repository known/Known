namespace Sample.Client.Pages.BizApply;

//业务申请列表
[Route("/bas/applies")]
public class BaApplyList : BaseTablePage<TbApply>
{
    private IApplyService Service;

    protected override async Task OnPageInitAsync()
    {
        await base.OnPageInitAsync();
        Service = await CreateServiceAsync<IApplyService>();
        
        //添加列表状态标签
        Table.Tab.AddTab("待审核");
        Table.Tab.AddTab("已审核");
        Table.Tab.OnChange = async t =>
        {
            UI.Info(t);
            await RefreshAsync();
        };
        Table.FormType = typeof(ApplyForm);
        Table.RowActions = row => Table.GetFlowRowActions(row); //根据数据状态显示操作按钮
        Table.OnQuery = criteria => Service.QueryApplysAsync(criteria);
		Table.Column(c => c.BizStatus).Template((b, r) => b.Tag(r.BizStatus));//自定义状态列
    }

    //新增按钮事件
    public async void New()
    {
        var row = await Service.GetDefaultApplyAsync(ApplyType.Test.ToString());
        Table.NewForm(Service.SaveApplyAsync, row);
    }

    //编辑操作
    public void Edit(TbApply row) => Table.EditForm(Service.SaveApplyAsync, row);
    //删除操作
    public void Delete(TbApply row) => Table.Delete(Service.DeleteApplysAsync, row);
    //批量删除操作
    public void DeleteM() => Table.DeleteM(Service.DeleteApplysAsync);
    //提交审核
    public void Submit(TbApply row) => this.SubmitFlow(row);
    //撤回
    public void Revoke(TbApply row) => this.RevokeFlow(row);
}