namespace Known.Cells;

/// <summary>
/// Excel操作接口。
/// </summary>
public interface IExcel : IDisposable
{
    /// <summary>
    /// 创建一个Sheet对象。
    /// </summary>
    /// <param name="name">Sheet名称。</param>
    /// <returns>Sheet对象。</returns>
    ISheet CreateSheet(string name);

    /// <summary>
    /// 根据序号获取Sheet对象。
    /// </summary>
    /// <param name="index">Sheet序号。</param>
    /// <returns>Sheet对象。</returns>
    ISheet GetSheet(int index);

    /// <summary>
    /// 根据名称获取Sheet对象。
    /// </summary>
    /// <param name="name">Sheet名称。</param>
    /// <returns>Sheet对象。</returns>
    ISheet GetSheet(string name);

    /// <summary>
    /// 将Sheet数据转换成文本字符串。
    /// </summary>
    /// <param name="index">Sheet序号。</param>
    /// <param name="headRow">数据标题行号，默认0。</param>
    /// <returns>文本字符串。</returns>
    string SheetToText(int index, int headRow = 0);

    /// <summary>
    /// 将Sheet数据转换成文本字符串。
    /// </summary>
    /// <param name="name">Sheet名称。</param>
    /// <param name="headRow">数据标题行号，默认0。</param>
    /// <returns>文本字符串。</returns>
    string SheetToText(string name, int headRow = 0);

    /// <summary>
    /// 将Sheet数据转换成字典列表。
    /// </summary>
    /// <param name="index">Sheet序号。</param>
    /// <param name="headRow">数据标题行号，默认0。</param>
    /// <returns>字典列表。</returns>
    List<Dictionary<string, string>> SheetToDictionaries(int index, int headRow = 0);

    /// <summary>
    /// 将Sheet数据转换成字典列表。
    /// </summary>
    /// <param name="name">Sheet名称。</param>
    /// <param name="headRow">数据标题行号，默认0。</param>
    /// <returns>字典列表。</returns>
    List<Dictionary<string, string>> SheetToDictionaries(string name, int headRow = 0);

    /// <summary>
    /// 将Excel保存到内存流中。
    /// </summary>
    /// <returns>内存流。</returns>
    MemoryStream SaveToStream();

    /// <summary>
    /// 将Excel保存为PDF，存到内存流中。
    /// </summary>
    /// <returns>内存流。</returns>
    MemoryStream SaveToPdfStream();

    /// <summary>
    /// 将Excel保存到文件中。
    /// </summary>
    /// <param name="fileName">文件路径。</param>
    void SaveAs(string fileName);

    /// <summary>
    /// 将Excel保存为PDF，存到文件中。
    /// </summary>
    /// <param name="fileName">文件路径。</param>
    void SaveAsPdf(string fileName);
}