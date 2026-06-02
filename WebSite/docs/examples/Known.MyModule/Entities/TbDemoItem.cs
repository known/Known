namespace Known.MyModule.Entities;

public class TbDemoItem : EntityBase
{
    [Required]
    [MaxLength(50)]
    [Column(Width = 150, IsQuery = true, IsViewLink = true)]
    [DisplayName("项目编码")]
    public string Code { get; set; }

    [Required]
    [MaxLength(100)]
    [Column(Width = 180, IsQuery = true)]
    [DisplayName("项目名称")]
    public string Name { get; set; }

    [Column]
    [DisplayName("备注")]
    public string Note { get; set; }
}
