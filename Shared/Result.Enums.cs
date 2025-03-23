namespace Known;

/// <summary>
/// 查询条件操作类型枚举。
/// </summary>
public enum QueryType
{
    /// <summary>
    /// 等于。
    /// </summary>
    Equal,
    /// <summary>
    /// 不等于。
    /// </summary>
    NotEqual,
    /// <summary>
    /// 小于。
    /// </summary>
    LessThan,
    /// <summary>
    /// 小于等于。
    /// </summary>
    LessEqual,
    /// <summary>
    /// 大于。
    /// </summary>
    GreatThan,
    /// <summary>
    /// 大于等于。
    /// </summary>
    GreatEqual,
    /// <summary>
    /// 区间(含两者)。
    /// </summary>
    Between,
    /// <summary>
    /// 区间(不含两者)。
    /// </summary>
    BetweenNotEqual,
    /// <summary>
    /// 区间(仅含前者)。
    /// </summary>
    BetweenLessEqual,
    /// <summary>
    /// 区间(仅含后者)。
    /// </summary>
    BetweenGreatEqual,
    /// <summary>
    /// 包含于。
    /// </summary>
    Contain,
    /// <summary>
    /// 不包含于。
    /// </summary>
    NotContain,
    /// <summary>
    /// 开头于。
    /// </summary>
    StartWith,
    /// <summary>
    /// 不开头于。
    /// </summary>
    NotStartWith,
    /// <summary>
    /// 结尾于。
    /// </summary>
    EndWith,
    /// <summary>
    /// 不结尾于。
    /// </summary>
    NotEndWith,
    /// <summary>
    /// 批量(逗号分割)。
    /// </summary>
    Batch
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