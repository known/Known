using Aspose.Cells;
using System.Collections.Generic;

namespace Known.Files
{
    public class AsposeExcel : IExcel
    {
        private Workbook wb;

        public AsposeExcel()
        {
            var lic = new License();
            lic.SetLicense("License.xml");
            wb = new Workbook();
            wb.Worksheets.Clear();

            Sheets = new List<ISheet>();
            foreach (Worksheet sheet in wb.Worksheets)
            {
                AddSheet(sheet.Name);
            }
        }

        public AsposeExcel(string fileName)
        {
            var lic = new License();
            lic.SetLicense("License.xml");
            wb = new Workbook(fileName);
            wb.CalculateFormula();

            Sheets = new List<ISheet>();
            foreach (Worksheet sheet in wb.Worksheets)
            {
                AddSheet(sheet.Name);
            }
        }

        public IList<ISheet> Sheets { get; }

        public ISheet AddSheet(string name)
        {
            var sheet = new AsposeSheet(wb, name);
            Sheets.Add(sheet);
            return sheet;
        }

        public void Save(string fileName)
        {
            wb.Save(fileName);
        }

        public void Save(string fileName, SavedFormat format)
        {
            var wbFormat = GetSaveFormat(format);
            wb.Save(fileName, wbFormat);
        }

        private SaveFormat GetSaveFormat(SavedFormat format)
        {
            switch (format)
            {
                case Files.SavedFormat.Auto:
                    return SaveFormat.Auto;
                case Files.SavedFormat.CSV:
                    return SaveFormat.CSV;
                case Files.SavedFormat.Html:
                    return SaveFormat.Html;
                case Files.SavedFormat.Pdf:
                    return SaveFormat.Pdf;
                case Files.SavedFormat.XPS:
                    return SaveFormat.XPS;
                case Files.SavedFormat.TIFF:
                    return SaveFormat.TIFF;
                case Files.SavedFormat.SVG:
                    return SaveFormat.SVG;
                default:
                    return SaveFormat.Auto;
            }
        }
    }
}
