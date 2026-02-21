namespace Known.Sample.Entities;

public class TbWork : EntityBase
{
    [Required]
    [MaxLength(50)]
    [Column(Width = 120, IsQuery = true, IsViewLink = true)]
    [DisplayName("工单编号")]
    public string WorkNo { get; set; }

    [Required]
    [MaxLength(50)]
    [Column(Width = 90)]
    [Category(nameof(WorkStatus))]
    [DisplayName("状态")]
    public string Status { get; set; }

    [Required]
    [MaxLength(50)]
    [Column(Width = 120)]
    [DisplayName("客户料号")]
    public string CustGNo { get; set; }

    [Column]
    [DisplayName("备注")]
    public string Note { get; set; }

    public Dictionary<string, object> PackInfo { get; set; }

    public virtual List<PackFieldInfo> PackFields { get; set; }
}