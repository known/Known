namespace Known.Sample.Pages.Test;

[Route("/test/orders")]
[Menu(AppConstant.Test, "订单列表", "bars", 1)]
public class OrderList : BaseTablePage<OrderInfo>
{
    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();
        //Table.ShowPager = false;
        Table.OnQuery = OrderData.QueryOrdersAsync;
    }

    [Action] public void New() { }

    [Action]
    public void Detail(OrderInfo row)
    {
        var model = new DialogModel
        {
            Title = $"订单明细 - {row.OrderNo}",
            Width = 1200,
            Content = b => b.Component<OrderDetail>().Set(c => c.IsDialog, true).Build()
        };
        UI.ShowDialog(model);
    }

    [Action(Group = "Export")] public Task ExportSelect() => Table.ExportDataAsync(ExportMode.Select);
    [Action(Group = "Export")] public Task ExportPage() => Table.ExportDataAsync(ExportMode.Page);
    [Action(Group = "Export")] public Task ExportQuery() => Table.ExportDataAsync(ExportMode.Query);
    [Action(Group = "Export")] public Task ExportAll() => Table.ExportDataAsync(ExportMode.All);
}