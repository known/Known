using System.Data;
using System.Linq;

namespace Known.Files
{
    public class Excel
    {
        private string fileName;

        public Excel(IExcel provider)
        {
            Provider = provider;
            Sheets = new SheetCollection(this);
        }

        public Excel(IExcel provider, string fileName)
        {
            this.fileName = fileName;
            Provider = provider;
            Sheets = new SheetCollection(this);
        }

        public IExcel Provider { get; }
        public SheetCollection Sheets { get; }

        public Sheet First
        {
            get { return Sheets.InnerSheets.FirstOrDefault(); }
        }

        public Sheet Last
        {
            get { return Sheets.InnerSheets.LastOrDefault(); }
        }

        public DataSet ExportDataSet(bool asString = true)
        {
            var ds = new DataSet();
            foreach (Sheet sheet in Sheets)
            {
                ds.Tables.Add(sheet.ExportData(0, asString));
            }
            return ds;
        }

        public void ImportDataSet(DataSet dataSet)
        {
            if (dataSet == null || dataSet.Tables.Count == 0)
                return;

            foreach (DataTable table in dataSet.Tables)
            {
                var sheet = Sheets.Add(table.TableName);
                sheet.ImportData(table, true, 0, 0);
            }
        }

        public void Save()
        {
            Provider.Save(fileName);
        }

        public void SaveAs(string fileName)
        {
            Utils.EnsureFile(fileName);
            Provider.Save(fileName);
        }

        public void SaveAsPdf(string fileName)
        {
            Utils.EnsureFile(fileName);
            Provider.Save(fileName, SavedFormat.Pdf);
        }
    }
}
