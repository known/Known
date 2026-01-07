namespace Known.Sample.Models;

public class OrderData
{
    public static async Task<PagingResult<OrderInfo>> QueryOrdersAsync(PagingCriteria criteria)
    {
        var items = Enumerable.Range(1, 100).Select(index => new OrderInfo
        {
            OrderNo = $"{DateTime.Now:yyyyMM}{index:0000}",
            OrderDate = DateTime.Now,
            Amount = 5000
        }).ToList();
        return items.ToPagingResult(criteria);
    }

    public static async Task<PagingResult<OrderDetailInfo>> QueryOrderDetailsAsync(PagingCriteria criteria)
    {
        var items = Enumerable.Range(1, 5).Select(index => new OrderDetailInfo
        {
            OrderNo = $"{DateTime.Now:yyyyMM}{index:0000}",
            OrderDate = DateTime.Now,
            SeqNo = index,
            GName = $"测试商品{index}",
            Qty = 10,
            Price = 100,
            Amount = 1000
        }).ToList();
        return items.ToPagingResult(criteria);
    }
}