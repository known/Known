namespace Known.Test.Pages.Samples.Models;

class DmTest : EntityBase
{
    [Column("编号")] public string No { get; set; }
    [Column("名称")] public string Name { get; set; }
    [Column("标题")] public string Title { get; set; }
    [Column("状态")] public string Status { get; set; }
    [Column("时间")] public DateTime Time { get; set; }
    [Column("图片")] public string Picture { get; set; }
    [Column("图标")] public string Icon { get; set; }
    [Column("颜色")] public string Color { get; set; }
    [Column("进度")] public decimal? Progress { get; set; }
    [Column("顺序")] public int Sort { get; set; }
    [Column("启用")] public bool Enabled { get; set; }
    [Column("备注")] public string Note { get; set; }
}