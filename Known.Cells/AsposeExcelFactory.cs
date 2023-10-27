namespace Known.Cells;

class AsposeExcelFactory : IExcelFactory
{
    public IExcel Create() => new AsposeExcel();
    public IExcel Create(string fileName) => new AsposeExcel(fileName);
    public IExcel Create(Stream stream) => new AsposeExcel(stream);
}