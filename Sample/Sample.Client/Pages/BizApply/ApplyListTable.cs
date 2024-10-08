namespace Sample.Client.Pages.BizApply;

class ApplyListTable : BaseTable<TbApplyList>
{
    private IApplyService Service;

    [Parameter] public TbApply Head { get; set; }

    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        Service = await CreateServiceAsync<IApplyService>();
        Table.ShowPager = true;
        Table.OnQuery = QueryApplyListsAsync;
        Table.FormType = typeof(ApplyListForm);
        Table.Form = new FormInfo { Width = 500 };
        if (!ReadOnly)
        {
            Table.Toolbar.AddAction(nameof(New));
            Table.Toolbar.AddAction(nameof(DeleteM));
            Table.SelectType = TableSelectType.Checkbox;
        }
        Table.AddColumn(c => c.Item, true).Width(180);
        Table.AddColumn(c => c.Note).Width(200);
        if (!ReadOnly)
        {
            Table.AddAction(nameof(Edit));
            Table.AddAction(nameof(Delete));
        }
    }

    public void New()
    {
        if (Head == null || Head.IsNew)
        {
            UI.Error("请先保存表头信息再添加明细！");
            return;
        }

        var row = new TbApplyList { HeadId = Head?.Id };
        Table.NewForm(Service.SaveApplyListAsync, row);
    }

    public void DeleteM() => Table.DeleteM(Service.DeleteApplyListsAsync);
    public void Edit(TbApplyList row) => Table.EditForm(Service.SaveApplyListAsync, row);
    public void Delete(TbApplyList row) => Table.Delete(Service.DeleteApplyListsAsync, row);

    private Task<PagingResult<TbApplyList>> QueryApplyListsAsync(PagingCriteria criteria)
    {
        criteria.SetQuery(nameof(TbApplyList.HeadId), QueryType.Equal, Head?.Id ?? "0");
        return Service.QueryApplyListsAsync(criteria);
    }
}