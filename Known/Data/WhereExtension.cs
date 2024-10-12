namespace Known.Data;

/// <summary>
/// Where条件扩展类。
/// </summary>
public static class WhereExtension
{
    /// <summary>
    /// 相当于 in 查询语句。
    /// </summary>
    /// <typeparam name="T">字段属性类型。</typeparam>
    /// <param name="field">字段属性。</param>
    /// <param name="array">In参数数组。</param>
    /// <returns></returns>
    public static bool In<T>(this T field, T[] array) => true;

    /// <summary>
    /// 相当于 not in 查询语句。
    /// </summary>
    /// <typeparam name="T">字段属性类型。</typeparam>
    /// <param name="field">字段属性。</param>
    /// <param name="array">NotIn参数数组。</param>
    /// <returns></returns>
    public static bool NotIn<T>(this T field, T[] array) => true;
}