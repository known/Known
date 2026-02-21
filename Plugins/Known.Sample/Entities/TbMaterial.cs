namespace Known.Sample.Entities;

public class TbMaterial : EntityBase
{
    [Required]
    [MaxLength(50)]
    [Column(Width = 150, IsQuery = true, IsViewLink = true)]
    [DisplayName("客户料号")]
    public string CustGNo { get; set; }

    [Column]
    [DisplayName("备注")]
    public string Note { get; set; }

    public List<PackFieldInfo> PackFields { get; set; } = [];
}