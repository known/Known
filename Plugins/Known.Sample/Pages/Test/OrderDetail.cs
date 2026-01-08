namespace Known.Sample.Pages.Test;

[Route("/test/order-details")]
[Menu(AppConstant.Test, "订单明细表", "table", 2)]
public class OrderDetail : BaseTablePage<OrderDetailInfo>
{
    [Parameter] public bool IsDialog { get; set; }

    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();
        Table.AutoHeight = !IsDialog;
        Table.TableTemplate = (b, m) => b.Component<MyTable<OrderDetailInfo>>().Set(c => c.Model, m).Build();
        Table.OnQuery = OrderData.QueryOrderDetailsAsync;
    }

    [Action(Group = "Export")] public Task ExportSelect() => Table.ExportDataAsync(ExportMode.Select);
    [Action(Group = "Export")] public Task ExportPage() => Table.ExportDataAsync(ExportMode.Page);
    [Action(Group = "Export")] public Task ExportQuery() => Table.ExportDataAsync(ExportMode.Query);
    [Action(Group = "Export")] public Task ExportAll() => Table.ExportDataAsync(ExportMode.All);
}