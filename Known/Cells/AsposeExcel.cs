using System.Collections.Generic;
using System.IO;
using Aspose.Cells;

namespace Known.Cells
{
    /// <summary>
    /// Aspose 组件实现的 Excel 操作类。
    /// </summary>
    public class AsposeExcel : IExcel
    {
        private Workbook wb;

        /// <summary>
        /// 创建一个 Aspose 组件实现的 Excel 操作类实例。
        /// </summary>
        public AsposeExcel()
        {
            var lic = new License();
            lic.SetLicense("License.xml");
            wb = new Workbook();
            wb.Worksheets.Clear();
            Sheets = new List<ISheet>();
        }

        /// <summary>
        /// 取得所有 Sheet 集合。
        /// </summary>
        public IList<ISheet> Sheets { get; }

        /// <summary>
        /// 打开指定文件路径的 Excel 文件。
        /// </summary>
        /// <param name="fileName">Excel 文件路径。</param>
        public void Open(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return;

            wb = new Workbook(fileName);
            wb.CalculateFormula();

            foreach (Worksheet sheet in wb.Worksheets)
            {
                AddSheet(sheet.Name);
            }
        }

        /// <summary>
        /// 打开指定的 Excel 文件流。
        /// </summary>
        /// <param name="stream">Excel 文件流。</param>
        public void Open(Stream stream)
        {
            if (stream == null)
                return;

            wb = new Workbook(stream);
            wb.CalculateFormula();

            foreach (Worksheet sheet in wb.Worksheets)
            {
                AddSheet(sheet.Name);
            }
        }

        /// <summary>
        /// 向 Excel 中添加一个 Sheet 页。
        /// </summary>
        /// <param name="name">Sheet 页名称。</param>
        /// <returns>添加的 Sheet 页对象。</returns>
        public ISheet AddSheet(string name)
        {
            var sheet = new AsposeSheet(wb, name);
            Sheets.Add(sheet);
            return sheet;
        }

        /// <summary>
        /// 删除指定名称的 Sheet 页。
        /// </summary>
        /// <param name="name">Sheet 页名称。</param>
        public void DeleteSheet(string name)
        {
            wb.Worksheets.RemoveAt(name);
        }

        /// <summary>
        /// 保存指定文件路径的 Excel 文件。
        /// </summary>
        /// <param name="fileName"></param>
        public void Save(string fileName)
        {
            wb.Save(fileName);
        }

        /// <summary>
        /// 保存指定文件路径和格式的 Excel 文件。
        /// </summary>
        /// <param name="fileName">文件路径。</param>
        /// <param name="format">保存格式。</param>
        public void Save(string fileName, SavedFormat format)
        {
            var wbFormat = GetSaveFormat(format);
            wb.Save(fileName, wbFormat);
        }

        /// <summary>
        /// 将 Excel 保存到文件流。
        /// </summary>
        /// <returns>Excel 文件流。</returns>
        public MemoryStream SaveToStream()
        {
            return wb.SaveToStream();
        }

        /// <summary>
        /// 将 Excel 保存到指定格式的文件流。
        /// </summary>
        /// <param name="format">保存格式。</param>
        /// <returns>指定格式的文件流。</returns>
        public MemoryStream SaveToStream(SavedFormat format)
        {
            var wbFormat = GetSaveFormat(format);
            var stream = new MemoryStream();
            wb.Save(stream, wbFormat);
            return stream;
        }

        /// <summary>
        /// 计算 Excel 中的公式。
        /// </summary>
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
