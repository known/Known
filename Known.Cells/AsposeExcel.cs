using Aspose.Cells;

namespace Known.Cells;

public class AsposeExcel : IExcel, IDisposable
{
    public AsposeExcel()
    {
        Workbook = new Workbook();
        Workbook.Worksheets.Clear();
    }

    public AsposeExcel(string fileName)
    {
        Workbook = new Workbook(fileName);
    }

    public AsposeExcel(Stream stream)
    {
        Workbook = new Workbook(stream);
    }

    internal Workbook Workbook { get; }

    public ISheet CreateSheet(string name)
    {
        var sheet = Workbook.Worksheets.Add(name);
        return new AsposeSheet(sheet);
    }

    public ISheet GetSheet(int index)
    {
        var sheet = Workbook.Worksheets[index];
        return new AsposeSheet(sheet);
    }

    public ISheet GetSheet(string name)
    {
        var sheet = Workbook.Worksheets[name];
        return new AsposeSheet(sheet);
    }

    public string SheetToText(int index, int headRow = 0)
    {
        var sheet = Workbook.Worksheets[index];
        if (sheet == null)
            return string.Empty;

        return SheetToText(sheet, headRow);
    }

    public string SheetToText(string name, int headRow = 0)
    {
        var sheet = Workbook.Worksheets[name];
        if (sheet == null)
            return string.Empty;

        return SheetToText(sheet, headRow);
    }

    private static string SheetToText(Worksheet sheet, int headRow)
    {
        var cells = sheet.Cells;
        if (cells.MaxDataRow == 0)
            return string.Empty;

        var lists = new List<string>();
        for (int i = headRow; i <= cells.MaxDataRow; i++)
        {
            var values = new List<string>();
            for (int j = 0; j <= cells.MaxDataColumn; j++)
            {
                var cell = cells[i, j];
                values.Add(cell == null ? "" : cell.StringValue);
            }
            lists.Add(string.Join("\t", values));
        }
        return string.Join(Environment.NewLine, lists);
    }

    public List<Dictionary<string, string>> SheetToDictionaries(int index, int headRow = 0)
    {
        var sheet = Workbook.Worksheets[index];
        if (sheet == null)
            return null;

        return SheetToDictionaries(sheet, headRow);
    }

    public List<Dictionary<string, string>> SheetToDictionaries(string name, int headRow = 0)
    {
        var sheet = Workbook.Worksheets[name];
        if (sheet == null)
            return null;

        return SheetToDictionaries(sheet, headRow);
    }

    private static List<Dictionary<string, string>> SheetToDictionaries(Worksheet sheet, int headRow)
    {
        var cells = sheet.Cells;
        if (cells.MaxDataRow == 0)
            return null;

        var head = cells.Rows[headRow];
        var lists = new List<Dictionary<string, string>>();
        for (int i = headRow + 1; i <= cells.MaxDataRow; i++)
        {
            var dic = new Dictionary<string, string>();
            for (int j = 0; j <= cells.MaxDataColumn; j++)
            {
                var key = head[j].StringValue;
                if (!string.IsNullOrWhiteSpace(key))
                {
                    var cell = cells[i, j];
                    dic[key] = cell == null ? "" : cell.StringValue;
                }
            }
            lists.Add(dic);
        }
        return lists;
    }

    public MemoryStream SaveToStream()
    {
        Workbook.CalculateFormula();
        var stream = new MemoryStream();
        Workbook.Save(stream, SaveFormat.Xlsx);
        return stream;
    }

    public MemoryStream SaveToPdfStream()
    {
        Workbook.CalculateFormula();
        var stream = new MemoryStream();
        Workbook.Save(stream, SaveFormat.Pdf);
        return stream;
    }

    public void SaveAs(string fileName)
    {
        var fi = new FileInfo(fileName);
        if (!fi.Directory.Exists)
            fi.Directory.Create();

        Workbook.CalculateFormula();
        Workbook.Save(fileName);
    }

    public void SaveAsPdf(string fileName)
    {
        var fi = new FileInfo(fileName);
        if (!fi.Directory.Exists)
            fi.Directory.Create();

        Workbook.CalculateFormula();
        Workbook.Save(fileName, SaveFormat.Pdf);
    }

    public void Dispose()
    {
        if (Workbook != null)
            Workbook.Dispose();
    }
}