namespace Known.Test.Pages.Samples.Models;

class DmBill : EntityBase
{
    [Column("单据编号", true)] public string BillNo { get; set; }
    [Column("单据日期", true)] public DateTime BillDate { get; set; }
    [Column("付款类型")] public string PayType { get; set; }
    [Column("总金额")] public string TotalAmount { get; set; }
    [Column("实收金额", true)] public string PaidAmount { get; set; }
    [Column("备注")] public string Note { get; set; }

    public virtual List<DmGoods> Lists { get; set; }
}