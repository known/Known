namespace Known.Entities;

/// <summary>
/// 数据字典实体类。
/// </summary>
public class SysDictionary : EntityBase
{
    /// <summary>
    /// 取得或设置类别。
    /// </summary>
    [Column("类别", "", true, "1", "50", IsGrid = true, IsQuery = true, CodeType = Constants.DicCategory, IsQueryAll = false)]
    public string Category { get; set; }

    /// <summary>
    /// 取得或设置类别名称。
    /// </summary>
    [Column("类别名称", "", true, "1", "50")]
    public string CategoryName { get; set; }

    /// <summary>
    /// 取得或设置代码。
    /// </summary>
    [Column("代码", "", true, "1", "100", IsGrid = true, IsQuery = true, IsForm = true, IsViewLink = true)]
    public string Code { get; set; }

    /// <summary>
    /// 取得或设置名称。
    /// </summary>
    [Column("名称", "", false, "1", "250", IsGrid = true, IsQuery = true, IsForm = true)]
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置顺序。
    /// </summary>
    [Column("顺序", "", true, IsGrid = true, IsForm = true)]
    public int Sort { get; set; }

    /// <summary>
    /// 取得或设置状态。
    /// </summary>
    [Column("状态", "", true, IsGrid = true, IsForm = true)]
    public bool Enabled { get; set; }

    /// <summary>
    /// 取得或设置备注。
    /// </summary>
    [Column("备注", "", false, "1", "500", IsGrid = true, IsForm = true)]
    public string Note { get; set; }

    /// <summary>
    /// 取得或设置子字典。
    /// </summary>
    [Column("子字典", "", false)]
    public string Child { get; set; }

    public virtual bool HasChild { get; set; }
    public virtual List<CodeName> Children => Utils.FromJson<List<CodeName>>(Child);
}