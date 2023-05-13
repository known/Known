namespace Known.Core.Cells;

public class ExcelFactory
{
    public static Excel Create() => new();
    public static Excel Create(string fileName) => new(fileName);
    public static Excel Create(Stream stream) => new(stream);
}