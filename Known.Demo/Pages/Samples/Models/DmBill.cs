namespace Known.Demo.Pages.Samples.Models;

class DmBill : EntityBase
{
    [Column("单据编号", true)] public string BillNo { get; set; }
    [Column("单据日期", true)] public DateTime BillDate { get; set; }
    [Column("付款类型")] public string PayType { get; set; }
    [Column("总金额")] public decimal? TotalAmount { get; set; }
    [Column("实收金额", true)] public decimal? PaidAmount { get; set; }
    [Column("备注")] public string Note { get; set; }

    public virtual List<DmGoods> Lists { get; set; }

    internal static DmBill LoadDefault()
    {
        var bill = new DmBill
        {
            BillNo = $"B{DateTime.Now:yyyyMM}00001",
            BillDate = DateTime.Now,
            PayType = "现金",
            Lists = new List<DmGoods>()
        };
        for (int i = 0; i < 3; i++)
        {
            bill.Lists.Add(new DmGoods
            {
                Code = $"G{i:0000}",
                Name = $"测试商品名称{i}",
                Model = $"测试商品规格型号{i}",
                TaxRate = 0.13M,
                MinQty = 100,
                MaxQty = 1000
            });
        }
        bill.TotalAmount = bill.Lists.Sum(l => l.MaxQty);
        bill.PaidAmount = bill.TotalAmount;
        return bill;
    }
}