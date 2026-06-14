namespace Known.Sample.Entities;

[Table("TB_TEST_MK")]
public class TbTestMK : BaseEntity
{
    [Key]
    [Column(Field = "FORM_NO")]
    public string FormNo { get; set; }

    [Key]
    [Column(Field = "SEQ_NO")]
    public int SeqNo { get; set; }

    [Column(Field = "NAME")]
    public string Name { get; set; }

    [Column(Field = "NOTE")]
    public string Note { get; set; }
}