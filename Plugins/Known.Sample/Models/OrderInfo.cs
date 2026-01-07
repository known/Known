namespace Known.Sample.Models;

public class OrderInfo
{
    public int Id { get; set; }

    [Column(Width = 120, IsQuery = true)]
    [DisplayName("订单号")]
    public string OrderNo { get; set; }

    [Column(Width = 120, Type = FieldType.Date, IsQuery = true)]
    [DisplayName("日期")]
    public DateTime OrderDate { get; set; }

    [Column(Width = 120)]
    [DisplayName("金额")]
    public double Amount { get; set; }

    [Column]
    [DisplayName("备注")]
    public string Note { get; set; }
}

public class OrderDetailInfo
{
    public int Id { get; set; }

    [Column(Width = 120, IsQuery = true)]
    [DisplayName("订单号")]
    public string OrderNo { get; set; }

    [Column(Width = 120, Type = FieldType.Date, IsQuery = true)]
    [DisplayName("日期")]
    public DateTime OrderDate { get; set; }

    [Column(Width = 120)]
    [DisplayName("序号")]
    public int SeqNo { get; set; }

    [Column(Width = 120, IsQuery = true)]
    [DisplayName("商品名称")]
    public string GName { get; set; }

    [Column(Width = 100)]
    [DisplayName("数量")]
    public int Qty { get; set; }

    [Column(Width = 100)]
    [DisplayName("单价")]
    public double Price { get; set; }

    [Column(Width = 100)]
    [DisplayName("金额")]
    public double Amount { get; set; }

    [Column]
    [DisplayName("备注")]
    public string Note { get; set; }
}