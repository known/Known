using System.Data;
using System.IO;
using System.Linq;

namespace Known.Cells
{
    /// <summary>
    /// Excel 操作类。
    /// </summary>
    public class Excel
    {
        private readonly string fileName;

        /// <summary>
        /// 创建一个 Excel 操作类的实例。
        /// </summary>
        /// <param name="provider">Excel 操作接口提供者。</param>
        public Excel(IExcel provider = null) : this("", provider) { }

        /// <summary>
        /// 创建一个 Excel 操作类的实例。
        /// </summary>
        /// <param name="fileName">Excel 文件路径。</param>
        /// <param name="provider">Excel 操作接口提供者。</param>
        public Excel(string fileName, IExcel provider = null)
        {
            this.fileName = fileName;
            Provider = provider ?? new AsposeExcel();
            Provider.Open(fileName);
            Sheets = new SheetCollection(this);
        }

        /// <summary>
        /// 创建一个 Excel 操作类的实例。
        /// </summary>
        /// <param name="stream">Excel 文件流。</param>
        /// <param name="provider">Excel 操作接口提供者。</param>
        public Excel(Stream stream, IExcel provider = null)
        {
            Provider = provider ?? new AsposeExcel();
            Provider.Open(stream);
            Sheets = new SheetCollection(this);
        }

        /// <summary>
        /// 取得 Excel 操作接口提供者。
        /// </summary>
        public IExcel Provider { get; }

        /// <summary>
        /// 取得 Excel 所有 Sheet 页集合。
        /// </summary>
        public SheetCollection Sheets { get; }

        /// <summary>
        /// 取得第一个 Sheet 页对象。
        /// </summary>
        public Sheet First
        {
            get { return Sheets.InnerSheets.FirstOrDefault(); }
        }

        /// <summary>
        /// 取得最后一个 Sheet 页对象。
        /// </summary>
        public Sheet Last
        {
            get { return Sheets.InnerSheets.LastOrDefault(); }
        }

        /// <summary>
        /// 导出整个 Excel 所有 Sheet 的数据。
        /// </summary>
        /// <param name="asString">是否以字符格式导出。</param>
        /// <returns>数据集。</returns>
        public DataSet ExportDataSet(bool asString = true)
        {
            var ds = new DataSet();
            foreach (Sheet sheet in Sheets)
            {
                ds.Tables.Add(sheet.ExportData(0, asString));
            }
            return ds;
        }

        /// <summary>
        /// 将数据集导入到 Excel 中。
        /// </summary>
        /// <param name="dataSet">数据集。</param>
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

        /// <summary>
        /// 向 Excel 中添加一个 Sheet 页。
        /// </summary>
        /// <param name="name">Sheet 页名称。</param>
        /// <returns>添加的 Sheet 页对象。</returns>
        public Sheet AddSheet(string name)
        {
            var sheet = Provider.AddSheet(name);
            return new Sheet(sheet);
        }

        /// <summary>
        /// 删除指定名称的 Sheet 页。
        /// </summary>
        /// <param name="name">Sheet 页名称。</param>
        public void DeleteSheet(string name)
        {
            Provider.DeleteSheet(name);
        }

        /// <summary>
        /// 保存当前打开的 Excel。
        /// </summary>
        public void Save()
        {
            SaveAs(fileName);
        }

        /// <summary>
        /// 另存为当前打开的 Excel。
        /// </summary>
        /// <param name="fileName">要保存的文件路径。</param>
        public void SaveAs(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return;

            var fileInfo = new FileInfo(fileName);
            if (!fileInfo.Directory.Exists)
                fileInfo.Directory.Create();

            Provider.Save(fileName);
        }

        /// <summary>
        /// 另存为当前打开的 Excel，指定保存格式。
        /// </summary>
        /// <param name="fileName">要保存的文件路径。</param>
        /// <param name="format">保存格式。</param>
        public void SaveAs(string fileName, SavedFormat format)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return;

            var fileInfo = new FileInfo(fileName);
            if (!fileInfo.Directory.Exists)
                fileInfo.Directory.Create();

            Provider.Save(fileName, format);
        }

        /// <summary>
        /// 另存为当前打开的 Excel 为 Pdf。
        /// </summary>
        /// <param name="fileName"></param>
        public void SaveAsPdf(string fileName)
        {
            SaveAs(fileName, SavedFormat.Pdf);
        }

        /// <summary>
        /// 将当前打开的 Excel 保存到内存流。
        /// </summary>
        /// <returns>内存流。</returns>
        public MemoryStream SaveToStream()
        {
            return Provider.SaveToStream();
        }

        /// <summary>
        /// 将当前打开的 Excel 保存到内存流，指定保存格式。
        /// </summary>
        /// <param name="format">保存格式。</param>
        /// <returns>内存流。</returns>
        public MemoryStream SaveToStream(SavedFormat format)
        {
            return Provider.SaveToStream(format);
        }

        /// <summary>
        /// 计算 Excel 中的公式。
        /// </summary>
        public void CalculateFormula()
        {
            Provider.CalculateFormula();
        }
    }
}
