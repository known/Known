namespace Known.Test.Pages.Samples.Models;

class DmTest : EntityBase
{
    [Column("编号")] public string No { get; set; }
    [Column("名称")] public string Name { get; set; }
    [Column("标题")] public string Title { get; set; }
    [Column("状态")] public string Status { get; set; }
    [Column("时间")] public DateTime? Time { get; set; }
    [Column("图片")] public string Picture { get; set; }
    [Column("图标")] public string Icon { get; set; }
    [Column("颜色")] public string Color { get; set; }
    [Column("进度")] public decimal Progress { get; set; }
    [Column("顺序")] public int Sort { get; set; }
    [Column("启用")] public bool Enabled { get; set; }
    [Column("备注")] public string Note { get; set; }

    private static readonly string[] Icons = new string[] { "fa fa-toggle-on", "fa fa-user", "fa fa-list", "fa fa-file" };
    private static readonly string[] Statuses = new string[] { "暂存", "待审核", "已撤回", "审核通过", "审核退回" };
    private static readonly decimal[] Progresses = new decimal[] { 0.35M, 0.5M, 1, 0.8M, 0.75M };
    internal static DmTest RandomInfo(int id)
    {
        var rndColor = Utils.GetRandomColor();
        var color = Utils.ToHtml(rndColor);
        return new DmTest
        {
            Id = $"test{id}",
            IsNew = false,
            No = $"{id}",
            Name = $"测试名称数据{id}",
            Title = $"测试标题信息{id}",
            Status = Statuses.Random(),
            Picture = "/img/login.jpg",
            Icon = Icons.Random(),
            Color = color,
            Progress = Progresses.Random(),
            Sort = id + 1,
            Enabled = true,
            Note = "测试哥哥是个大帅哥山的"
        };
    }
}