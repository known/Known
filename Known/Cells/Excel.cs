namespace Known.Cells;

public interface IExcel : IDisposable
{
    ISheet CreateSheet(string name);
    ISheet GetSheet(int index);
    ISheet GetSheet(string name);
    string SheetToText(int index, int headRow = 0);
    string SheetToText(string name, int headRow = 0);
    List<Dictionary<string, string>> SheetToDictionaries(int index, int headRow = 0);
    List<Dictionary<string, string>> SheetToDictionaries(string name, int headRow = 0);
    MemoryStream SaveToStream();
    MemoryStream SaveToPdfStream();
    void SaveAs(string fileName);
    void SaveAsPdf(string fileName);
}