namespace Known.Cells;

/// <summary>
/// Excel对象工厂接口。
/// </summary>
public interface IExcelFactory
{
    /// <summary>
    /// 创建一个空的Excel对象。
    /// </summary>
    /// <returns>Excel对象。</returns>
    IExcel Create();

    /// <summary>
    /// 根据文件路径创建一个Excel对象。
    /// </summary>
    /// <param name="fileName">文件路径。</param>
    /// <returns>Excel对象。</returns>
    IExcel Create(string fileName);

    /// <summary>
    /// 根据文件流创建一个Excel对象。
    /// </summary>
    /// <param name="stream">文件流。</param>
    /// <returns>Excel对象。</returns>
    IExcel Create(Stream stream);
}

/// <summary>
/// Excel对象工厂类。
/// </summary>
public class ExcelFactory
{
    /// <summary>
    /// 取得或设置Excel对象工厂接口实例。
    /// </summary>
    public static IExcelFactory Factory { get; set; }

    /// <summary>
    /// 创建一个空的Excel对象。
    /// </summary>
    /// <returns>Excel对象。</returns>
    public static IExcel Create() => Factory?.Create();

    /// <summary>
    /// 根据文件路径创建一个Excel对象。
    /// </summary>
    /// <param name="fileName">文件路径。</param>
    /// <returns>Excel对象。</returns>
    public static IExcel Create(string fileName)
    {
        //用File读取流，再创建Excel实例，适配Docker环境
        var bytes = File.ReadAllBytes(fileName);
        var stream = new MemoryStream(bytes);
        return Create(stream);
    }

    /// <summary>
    /// 根据文件流创建一个Excel对象。
    /// </summary>
    /// <param name="stream">文件流。</param>
    /// <returns>Excel对象。</returns>
    public static IExcel Create(Stream stream) => Factory?.Create(stream);
}