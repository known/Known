using System;
using System.Data;
using System.Linq;

namespace Known.Files
{
    /// <summary>
    /// Excel类。
    /// </summary>
    public class Excel
    {
        private string fileName;

        /// <summary>
        /// 构造函数，创建一个Excel类实例。
        /// </summary>
        /// <param name="provider">Excel提供者接口。</param>
        public Excel(IExcel provider)
        {
            Provider = provider ?? throw new ArgumentNullException("provider");
            Sheets = new SheetCollection(this);
        }

        /// <summary>
        /// 构造函数，创建一个Excel类实例。
        /// </summary>
        /// <param name="provider">Excel提供者接口。</param>
        /// <param name="fileName">Excel文件路径。</param>
        public Excel(IExcel provider, string fileName)
        {
            this.fileName = fileName;
            Provider = provider ?? throw new ArgumentNullException("provider");
            Sheets = new SheetCollection(this);
        }

        /// <summary>
        /// 取得Excel提供者接口。
        /// </summary>
        public IExcel Provider { get; }

        /// <summary>
        /// 取得Excel所有Sheet集合。
        /// </summary>
        public SheetCollection Sheets { get; }

        /// <summary>
        /// 取得Excel第一个Sheet。
        /// </summary>
        public Sheet First
        {
            get { return Sheets.InnerSheets.FirstOrDefault(); }
        }

        /// <summary>
        /// 取得Excel最后一个Sheet。
        /// </summary>
        public Sheet Last
        {
            get { return Sheets.InnerSheets.LastOrDefault(); }
        }

        /// <summary>
        /// 导出Excel所有Sheet数据，返回DataSet。
        /// </summary>
        /// <param name="asString">单元格是否以字符串形式导出。</param>
        /// <returns>导出的DataSet。</returns>
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
        /// 导入DataSet数据至Excel。
        /// </summary>
        /// <param name="dataSet">DataSet数据。</param>
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
        /// 添加指定名称的Sheet并返回Sheet对象。
        /// </summary>
        /// <param name="name">Sheet名。</param>
        /// <returns>Sheet对象。</returns>
        public ISheet AddSheet(string name)
        {
            return Provider.AddSheet(name);
        }

        /// <summary>
        /// 删除指定名称的Sheet。
        /// </summary>
        /// <param name="name">Sheet名。</param>
        public void DeleteSheet(string name)
        {
            Provider.DeleteSheet(name);
        }

        /// <summary>
        /// 保存Excel。
        /// </summary>
        public void Save()
        {
            Provider.Save(fileName);
        }

        /// <summary>
        /// 另存Excel至指定文件路径。
        /// </summary>
        /// <param name="fileName">另存的文件路径。</param>
        public void SaveAs(string fileName)
        {
            Utils.EnsureFile(fileName);
            Provider.Save(fileName);
        }

        /// <summary>
        /// 另存Excel为指定格式的文件。
        /// </summary>
        /// <param name="fileName">另存的文件路径。</param>
        /// <param name="format">另存的文件格式。</param>
        public void SaveAs(string fileName, SavedFormat format)
        {
            Utils.EnsureFile(fileName);
            Provider.Save(fileName, format);
        }

        /// <summary>
        /// 另存Excel为指定的Pdf文件。
        /// </summary>
        /// <param name="fileName">另存的Pdf文件路径。</param>
        public void SaveAsPdf(string fileName)
        {
            SaveAs(fileName, SavedFormat.Pdf);
        }
    }
}
