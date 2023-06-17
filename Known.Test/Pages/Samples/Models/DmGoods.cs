namespace Known.Test.Pages.Samples.Models;

class DmGoods : EntityBase
{
    [Column("编码")] public string Code { get; set; }
    [Column("名称")] public string Name { get; set; }
    [Column("型号")] public string Model { get; set; }
    [Column("单位")] public string Unit { get; set; }
    [Column("增值税率")] public decimal? TaxRate { get; set; }
    [Column("库存下限")] public decimal? MinQty { get; set; }
    [Column("库存上限")] public decimal? MaxQty { get; set; }
    [Column("备注")] public string Note { get; set; }
    [Column("图片")] public string Picture { get; set; }

    public override string ToString() => $"{Code}-{Name}";
}