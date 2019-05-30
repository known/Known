using System.Collections.Generic;
using System.IO;

namespace Known.Cells
{
    /// <summary>
    /// Excel 操作接口。
    /// </summary>
    public interface IExcel
    {
        /// <summary>
        /// 取得所有 Sheet 集合。
        /// </summary>
        IList<ISheet> Sheets { get; }

        /// <summary>
        /// 打开指定文件路径的 Excel 文件。
        /// </summary>
        /// <param name="fileName">Excel 文件路径。</param>
        void Open(string fileName);

        /// <summary>
        /// 打开指定的 Excel 文件流。
        /// </summary>
        /// <param name="stream">Excel 文件流。</param>
        void Open(Stream stream);

        /// <summary>
        /// 向 Excel 中添加一个 Sheet 页。
        /// </summary>
        /// <param name="name">Sheet 页名称。</param>
        /// <returns>添加的 Sheet 页对象。</returns>
        ISheet AddSheet(string name);

        /// <summary>
        /// 删除指定名称的 Sheet 页。
        /// </summary>
        /// <param name="name">Sheet 页名称。</param>
        void DeleteSheet(string name);

        /// <summary>
        /// 保存指定文件路径的 Excel 文件。
        /// </summary>
        /// <param name="fileName"></param>
        void Save(string fileName);

        /// <summary>
        /// 保存指定文件路径和格式的 Excel 文件。
        /// </summary>
        /// <param name="fileName">文件路径。</param>
        /// <param name="format">保存格式。</param>
        void Save(string fileName, SavedFormat format);

        /// <summary>
        /// 将 Excel 保存到文件流。
        /// </summary>
        /// <returns>Excel 文件流。</returns>
        MemoryStream SaveToStream();

        /// <summary>
        /// 将 Excel 保存到指定格式的文件流。
        /// </summary>
        /// <param name="format">保存格式。</param>
        /// <returns>指定格式的文件流。</returns>
        MemoryStream SaveToStream(SavedFormat format);

        /// <summary>
        /// 计算 Excel 中的公式。
        /// </summary>
        void CalculateFormula();
    }
}
