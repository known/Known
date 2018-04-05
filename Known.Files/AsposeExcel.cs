using Aspose.Cells;
using System.Collections.Generic;

namespace Known.Files
{
    /// <summary>
    /// Aspose组件实现的Excel操作类。
    /// </summary>
    public class AsposeExcel : IExcel
    {
        private Workbook wb;

        /// <summary>
        /// 构造函数，创建一个Aspose组件实现的Excel类实例。
        /// </summary>
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

        /// <summary>
        /// 构造函数，创建一个Aspose组件实现的Excel类实例。
        /// </summary>
        /// <param name="fileName">Excel文件路径。</param>
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

        /// <summary>
        /// 取得Excel所有Sheet集合。
        /// </summary>
        public IList<ISheet> Sheets { get; }

        /// <summary>
        /// 添加指定名称的Sheet并返回Sheet对象。
        /// </summary>
        /// <param name="name">Sheet名。</param>
        /// <returns>Sheet对象。</returns>
        public ISheet AddSheet(string name)
        {
            var sheet = new AsposeSheet(wb, name);
            Sheets.Add(sheet);
            return sheet;
        }

        /// <summary>
        /// 保存Excel至指定的文件路径。
        /// </summary>
        /// <param name="fileName">指定的文件路径。</param>
        public void Save(string fileName)
        {
            wb.Save(fileName);
        }

        /// <summary>
        /// 保存Excel至指定格式的文件路径。
        /// </summary>
        /// <param name="fileName">指定的文件路径。</param>
        /// <param name="format">指定的文件格式。</param>
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
