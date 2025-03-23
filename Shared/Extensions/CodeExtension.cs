namespace Known.Extensions;

/// <summary>
/// 代码表扩展类。
/// </summary>
public static class CodeExtension
{
    /// <summary>
    /// 添加代码表数据。
    /// </summary>
    /// <param name="codes">代码表列表。</param>
    /// <param name="code">代码。</param>
    /// <param name="name">名称。</param>
    /// <param name="data">附加数据。</param>
    public static void Add(this List<CodeInfo> codes, string code, string name, object data = null)
    {
        codes.Add(new CodeInfo(code, name, data));
    }

    /// <summary>
    /// 添加代码表数据。
    /// </summary>
    /// <param name="codes">代码表列表。</param>
    /// <param name="category">类别。</param>
    /// <param name="code">代码。</param>
    /// <param name="name">名称。</param>
    /// <param name="data">附加数据。</param>
    public static void Add(this List<CodeInfo> codes, string category, string code, string name, object data = null)
    {
        codes.Add(new CodeInfo(category, code, name, data));
    }

    /// <summary>
    /// 往代码表列表中插入空文本字符串。
    /// </summary>
    /// <param name="codes">代码表列表。</param>
    /// <param name="emptyText">空文本字符串，默认空。</param>
    /// <returns>新代码表列表。</returns>
    public static List<CodeInfo> ToCodes(this List<CodeInfo> codes, string emptyText = "")
    {
        var infos = new List<CodeInfo>();
        if (!string.IsNullOrWhiteSpace(emptyText))
            infos.Add(new CodeInfo("", emptyText));

        if (codes != null && codes.Count > 0)
            infos.AddRange(codes);

        return infos;
    }
}