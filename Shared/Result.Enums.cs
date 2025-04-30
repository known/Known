namespace Known;

/// <summary>
/// 查询条件操作类型枚举。
/// </summary>
public enum QueryType
{
    /// <summary>
    /// 等于。
    /// </summary>
    [Display(Name = "等于")]
    Equal,
    /// <summary>
    /// 不等于。
    /// </summary>
    [Display(Name = "不等于")]
    NotEqual,
    /// <summary>
    /// 小于。
    /// </summary>
    [Display(Name = "小于")]
    LessThan,
    /// <summary>
    /// 小于等于。
    /// </summary>
    [Display(Name = "小于等于")]
    LessEqual,
    /// <summary>
    /// 大于。
    /// </summary>
    [Display(Name = "大于")]
    GreatThan,
    /// <summary>
    /// 大于等于。
    /// </summary>
    [Display(Name = "大于等于")]
    GreatEqual,
    /// <summary>
    /// 区间(含两者)。
    /// </summary>
    [Display(Name = "区间(含两者)")]
    Between,
    /// <summary>
    /// 区间(不含两者)。
    /// </summary>
    [Display(Name = "区间(不含两者)")]
    BetweenNotEqual,
    /// <summary>
    /// 区间(仅含前者)。
    /// </summary>
    [Display(Name = "区间(仅含前者)")]
    BetweenLessEqual,
    /// <summary>
    /// 区间(仅含后者)。
    /// </summary>
    [Display(Name = "区间(仅含后者)")]
    BetweenGreatEqual,
    /// <summary>
    /// 包含于。
    /// </summary>
    [Display(Name = "包含于")]
    Contain,
    /// <summary>
    /// 不包含于。
    /// </summary>
    [Display(Name = "不包含于")]
    NotContain,
    /// <summary>
    /// 开头于。
    /// </summary>
    [Display(Name = "开头于")]
    StartWith,
    /// <summary>
    /// 不开头于。
    /// </summary>
    [Display(Name = "不开头于")]
    NotStartWith,
    /// <summary>
    /// 结尾于。
    /// </summary>
    [Display(Name = "结尾于")]
    EndWith,
    /// <summary>
    /// 不结尾于。
    /// </summary>
    [Display(Name = "不结尾于")]
    NotEndWith,
    /// <summary>
    /// 批量(逗号分割)。
    /// </summary>
    [Display(Name = "批量(逗号分割)")]
    Batch,
    /// <summary>
    /// In(逗号分割)。
    /// </summary>
    [Display(Name = "In(逗号分割)")]
    In,
    /// <summary>
    /// NotIn(逗号分割)。
    /// </summary>
    [Display(Name = "NotIn(逗号分割)")]
    NotIn
}

/// <summary>
/// 导出模式枚举。
/// </summary>
public enum ExportMode
{
    /// <summary>
    /// 不导出。
    /// </summary>
    None,
    /// <summary>
    /// 导出当前页数据。
    /// </summary>
    Page,
    /// <summary>
    /// 导出查询结果数据。
    /// </summary>
    Query,
    /// <summary>
    /// 导出全部数据。
    /// </summary>
    All
}