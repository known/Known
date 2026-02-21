namespace Known.Sample.Pages.Produce;

[Route("/pms/works")]
[Menu(AppConstant.Produce, "生产工单", "bars", 2)]
public class WorkList : BaseTablePage<TbWork>
{
    private IProduceService Service;

    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();
        Service = await CreateServiceAsync<IProduceService>();

        Table.CurrentTab = WorkStatus.Pending;
        Table.AddTab(WorkStatus.Pending);
        Table.AddTab(WorkStatus.Producing);
        Table.AddTab(WorkStatus.Produced);
        Table.FormType = typeof(WorkForm);
        Table.Form = new FormInfo { Width = 900 };
        Table.OnQuery = QueryWorksAsync;
        Table.Column(c => c.Status).Tag();
    }

    [Action(Tabs = [WorkStatus.Pending])]
    public void New() => Table.NewForm(Service.SaveWorkAsync, new TbWork());

    [Action(Tabs = [WorkStatus.Pending])]
    public void Edit(TbWork row) => Table.EditForm(Service.SaveWorkAsync, row);

    [Action(Tabs = [WorkStatus.Pending])]
    public void Delete(TbWork row) => Table.Delete(Service.DeleteWorksAsync, row);

    [Action(Tabs = [WorkStatus.Pending])]
    public void DeleteM() => Table.DeleteM(Service.DeleteWorksAsync);

    [Action] public Task Print(TbWork row) => JS.PrintWorkAsync(row, CurrentUser.UserName);

    private Task<PagingResult<TbWork>> QueryWorksAsync(PagingCriteria criteria)
    {
        criteria.SetQuery(nameof(TbWork.Status), QueryType.Equal, Table.CurrentTab);
        return Service.QueryWorksAsync(criteria);
    }
}