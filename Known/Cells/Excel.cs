using System;
using System.Data;
using System.IO;
using System.Linq;

namespace Known.Cells
{
    public class Excel
    {
        private string fileName;

        public Excel(IExcel provider) : this(provider, null) { }

        public Excel(IExcel provider, string fileName)
        {
            this.fileName = fileName;
            Provider = provider ?? throw new ArgumentNullException(nameof(provider));
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

        public Sheet AddSheet(string name)
        {
            var sheet = Provider.AddSheet(name);
            return new Sheet(sheet);
        }

        public void DeleteSheet(string name)
        {
            Provider.DeleteSheet(name);
        }

        public void Save()
        {
            Provider.Save(fileName);
        }

        public void SaveAs(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return;

            var fileInfo = new FileInfo(fileName);
            if (!fileInfo.Directory.Exists)
                fileInfo.Directory.Create();

            Provider.Save(fileName);
        }

        public void SaveAs(string fileName, SavedFormat format)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return;

            var fileInfo = new FileInfo(fileName);
            if (!fileInfo.Directory.Exists)
                fileInfo.Directory.Create();

            Provider.Save(fileName, format);
        }

        public void SaveAsPdf(string fileName)
        {
            SaveAs(fileName, SavedFormat.Pdf);
        }

        public MemoryStream SaveToStream()
        {
            return Provider.SaveToStream();
        }

        public MemoryStream SaveToStream(SavedFormat format)
        {
            return Provider.SaveToStream(format);
        }

        public void CalculateFormula()
        {
            Provider.CalculateFormula();
        }
    }
}
