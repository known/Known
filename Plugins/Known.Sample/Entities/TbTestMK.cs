namespace Known.Sample.Entities;

public class TbTestMK : BaseEntity
{
    [Key] public string FormNo { get; set; }
    [Key] public int SeqNo { get; set; }
    public string Name { get; set; }
    public string Note { get; set; }
}