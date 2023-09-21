namespace Sample.Razor.Samples.Models;

class DmGoods : EntityBase
{
    [Column("编码")] public string Code { get; set; }
    [Column("名称", true)] public string Name { get; set; }
    [Column("型号")] public string Model { get; set; }
    [Column("单位")] public string Unit { get; set; }
    [Column("增值税率")] public decimal? TaxRate { get; set; }
    [Column("库存下限")] public decimal? MinQty { get; set; }
    [Column("库存上限")] public decimal? MaxQty { get; set; }
    [Column("备注")] public string Note { get; set; }
    [Column("图片")] public string Picture { get; set; }

    public override string ToString() => Code;

    private static readonly string[] Uints = new string[] { "个", "套", "项", "张", "台" };
    internal static DmGoods RandomInfo(int id)
    {
        return new DmGoods
        {
            Code = $"G{id:0000}",
            Name = $"测试商品名称{id}",
            Model = $"测试商品规格型号{id}",
            Unit = Uints.Random(),
            TaxRate = 0.13M,
            MinQty = 100,
            MaxQty = 1000,
            Note = $"测试商品备注{id}",
            Picture = "/img/login.jpg"
        };
    }
}