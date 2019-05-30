using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Known.Cells
{
    /// <summary>
    /// Sheet 页集合类。
    /// </summary>
    public class SheetCollection : IEnumerable
    {
        private readonly Excel excel;

        internal SheetCollection(Excel excel)
        {
            this.excel = excel ?? throw new ArgumentNullException(nameof(excel));
            InnerSheets = new List<Sheet>();
            foreach (var sheet in excel.Provider.Sheets)
            {
                Add(new Sheet(sheet));
            }
        }

        internal List<Sheet> InnerSheets { get; }

        /// <summary>
        /// 取得 Sheet 页集合的大小。
        /// </summary>
        public int Count
        {
            get { return excel.Provider.Sheets.Count; }
        }

        /// <summary>
        /// 取得指定序号的 Sheet 页对象。
        /// </summary>
        /// <param name="index">Sheet 页序号。</param>
        /// <returns>Sheet 页对象。</returns>
        public Sheet this[int index]
        {
            get { return InnerSheets.FirstOrDefault(s => s.Index == index); }
        }

        /// <summary>
        /// 取得指定名称的 Sheet 页对象。
        /// </summary>
        /// <param name="name">Sheet 页名称。</param>
        /// <returns>Sheet 页对象。</returns>
        public Sheet this[string name]
        {
            get { return InnerSheets.FirstOrDefault(s => s.Name == name); }
        }

        /// <summary>
        /// 向 Sheet 集合中添加一个 Sheet 页对象。
        /// </summary>
        /// <param name="name">Sheet 页名称。</param>
        /// <returns>添加的 Sheet 页对象。</returns>
        public Sheet Add(string name)
        {
            if (!Contains(name))
            {
                var sheet = excel.Provider.AddSheet(name);
                Add(new Sheet(sheet));
            }
            return this[name];
        }

        private void Add(Sheet sheet)
        {
            if (sheet == null)
                throw new ArgumentNullException("sheet");

            InnerSheets.Add(sheet);
        }

        /// <summary>
        /// 判断 Sheet 集合中是否包含指定名称的 Sheet 页对象。
        /// </summary>
        /// <param name="name">Sheet 页名称。</param>
        /// <returns>包含返回 True，否则返回 False。</returns>
        public bool Contains(string name)
        {
            return InnerSheets.Count(s => s.Name == name) > 0;
        }

        /// <summary>
        /// 获取 Sheet 页集合的迭代器。
        /// </summary>
        /// <returns>Sheet 页集合的迭代器。</returns>
        public IEnumerator GetEnumerator()
        {
            return InnerSheets.GetEnumerator();
        }
    }
}