using System.Collections.Generic;

namespace Known.Files
{
    /// <summary>
    /// Excel接口。
    /// </summary>
    public interface IExcel
    {
        /// <summary>
        /// 取得Excel所有Sheet集合。
        /// </summary>
        IList<ISheet> Sheets { get; }

        /// <summary>
        /// 添加指定名称的Sheet并返回Sheet对象。
        /// </summary>
        /// <param name="name">Sheet名。</param>
        /// <returns>Sheet对象。</returns>
        ISheet AddSheet(string name);

        /// <summary>
        /// 保存Excel至指定的文件路径。
        /// </summary>
        /// <param name="fileName">指定的文件路径。</param>
        void Save(string fileName);

        /// <summary>
        /// 保存Excel至指定格式的文件路径。
        /// </summary>
        /// <param name="fileName">指定的文件路径。</param>
        /// <param name="format">指定的文件格式。</param>
        void Save(string fileName, SavedFormat format);
    }
}
