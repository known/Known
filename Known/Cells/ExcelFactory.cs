namespace Known.Cells;

public interface IExcelFactory
{
    IExcel Create();
    IExcel Create(string fileName);
    IExcel Create(Stream stream);
}

public class ExcelFactory
{
    public static IExcelFactory Factory { get; set; }

    public static IExcel Create() => Factory?.Create();
    public static IExcel Create(string fileName) => Factory?.Create(fileName);
    public static IExcel Create(Stream stream) => Factory?.Create(stream);
}