using Aspose.Cells;
using System.Data;
using System.Linq;

namespace Known.Files
{
    public class Excel
    {
        private Workbook wb;
        private string fileName;

        public Excel()
        {
            var lic = new License();
            lic.SetLicense("License.xml");
            wb = new Workbook();
            wb.Worksheets.Clear();
            Sheets = new SheetCollection(this);
        }

        public Excel(string fileName)
        {
            this.fileName = fileName;
            var lic = new License();
            lic.SetLicense("License.xml");
            wb = new Workbook(fileName);
            wb.CalculateFormula();
            Sheets = new SheetCollection(this);
        }

        public SheetCollection Sheets { get; }

        internal Workbook Workbook
        {
            get { return wb; }
        }

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
            wb.Save(fileName);
        }

        public void SaveAs(string fileName)
        {
            Utils.EnsureFile(fileName);
            wb.Save(fileName);
        }

        public void SaveAsPdf(string fileName)
        {
            Utils.EnsureFile(fileName);
            wb.Save(fileName, SaveFormat.Pdf);
        }
    }
}
