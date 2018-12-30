using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Known.Cells
{
    public class SheetCollection : IEnumerable
    {
        private Excel excel;

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

        public int Count
        {
            get { return excel.Provider.Sheets.Count; }
        }

        public Sheet this[int index]
        {
            get { return InnerSheets.FirstOrDefault(s => s.Index == index); }
        }

        public Sheet this[string name]
        {
            get { return InnerSheets.FirstOrDefault(s => s.Name == name); }
        }

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

        public bool Contains(string name)
        {
            return InnerSheets.Count(s => s.Name == name) > 0;
        }

        public IEnumerator GetEnumerator()
        {
            return InnerSheets.GetEnumerator();
        }
    }
}