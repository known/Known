using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Known.Cells
{
    /// <summary>
    /// Sheet集合。
    /// </summary>
    public class SheetCollection : IEnumerable
    {
        private Excel excel;

        /// <summary>
        /// 构造函数，创建一个Sheet集合实例。
        /// </summary>
        /// <param name="excel">Excel对象。</param>
        internal SheetCollection(Excel excel)
        {
            this.excel = excel ?? throw new ArgumentNullException("excel");
            InnerSheets = new List<Sheet>();
            foreach (var sheet in excel.Provider.Sheets)
            {
                Add(new Sheet(sheet));
            }
        }

        /// <summary>
        /// 取得所有Sheet对象集合。
        /// </summary>
        internal List<Sheet> InnerSheets { get; }

        /// <summary>
        /// 取得所有Sheet的数量。
        /// </summary>
        public int Count
        {
            get { return excel.Provider.Sheets.Count; }
        }

        /// <summary>
        /// 添加一个Sheet。
        /// </summary>
        /// <param name="name">添加的Sheet名称。</param>
        /// <returns>添加的Sheet对象。</returns>
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
        /// 根据Sheet名称判断是否包含该名称的Sheet。
        /// </summary>
        /// <param name="name">Sheet名称。</param>
        /// <returns>包含该名称Sheet返回true，否则返回false。</returns>
        public bool Contains(string name)
        {
            return InnerSheets.Count(s => s.Name == name) > 0;
        }

        /// <summary>
        /// 获取Sheet集合枚举器。
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            return InnerSheets.GetEnumerator();
        }

        /// <summary>
        /// 根据索引取得集合中的Sheet对象。
        /// </summary>
        /// <param name="index">Sheet的索引位置。</param>
        /// <returns>Sheet对象。</returns>
        public Sheet this[int index]
        {
            get { return InnerSheets.FirstOrDefault(s => s.Index == index); }
        }

        /// <summary>
        /// 根据Sheet名称取得集合中的Sheet对象。
        /// </summary>
        /// <param name="name">Sheet名称。</param>
        /// <returns>Sheet对象。</returns>
        public Sheet this[string name]
        {
            get { return InnerSheets.FirstOrDefault(s => s.Name == name); }
        }
    }
}