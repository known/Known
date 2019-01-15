using System.Collections.Generic;
using System.IO;
using Aspose.Cells;

namespace Known.Cells
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

        public AsposeExcel(Stream stream)
        {
            var lic = new License();
            lic.SetLicense("License.xml");
            wb = new Workbook(stream);
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

        public void DeleteSheet(string name)
        {
            wb.Worksheets.RemoveAt(name);
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

        public MemoryStream SaveToStream()
        {
            return wb.SaveToStream();
        }

        public MemoryStream SaveToStream(SavedFormat format)
        {
            var wbFormat = GetSaveFormat(format);
            var stream = new MemoryStream();
            wb.Save(stream, wbFormat);
            return stream;
        }

        public void CalculateFormula()
        {
            wb.CalculateFormula();
        }

        private SaveFormat GetSaveFormat(SavedFormat format)
        {
            switch (format)
            {
                case Cells.SavedFormat.Auto:
                    return SaveFormat.Auto;
                case Cells.SavedFormat.CSV:
                    return SaveFormat.CSV;
                case Cells.SavedFormat.Html:
                    return SaveFormat.Html;
                case Cells.SavedFormat.Pdf:
                    return SaveFormat.Pdf;
                case Cells.SavedFormat.XPS:
                    return SaveFormat.XPS;
                case Cells.SavedFormat.TIFF:
                    return SaveFormat.TIFF;
                case Cells.SavedFormat.SVG:
                    return SaveFormat.SVG;
                default:
                    return SaveFormat.Auto;
            }
        }
    }
}
