using Aspose.Cells;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Known.Files
{
    public class SheetCollection : IEnumerable
    {
        private Excel excel;
        private List<Sheet> sheets;

        internal SheetCollection(Excel excel)
        {
            if (excel == null)
                throw new ArgumentNullException("excel");

            this.excel = excel;
            sheets = new List<Sheet>();
            foreach (Worksheet sheet in excel.Workbook.Worksheets)
            {
                Add(new Sheet(excel.Workbook, sheet.Name));
            }
        }

        internal List<Sheet> InnerSheets
        {
            get { return sheets; }
        }

        public int Count
        {
            get { return excel.Workbook.Worksheets.Count; }
        }

        public Sheet Add(string name)
        {
            if (!Contains(name))
            {
                excel.Workbook.Worksheets.Add(name);
                Add(new Sheet(excel.Workbook, name));
            }
            return this[name];
        }

        private void Add(Sheet sheet)
        {
            if (sheet == null)
                throw new ArgumentNullException("sheet");

            sheets.Add(sheet);
        }

        public bool Contains(string name)
        {
            return sheets.Count(s => s.Name == name) > 0;
        }

        public IEnumerator GetEnumerator()
        {
            return sheets.GetEnumerator();
        }

        public Sheet this[int index]
        {
            get { return sheets.FirstOrDefault(s => s.Index == index); }
        }

        public Sheet this[string name]
        {
            get { return sheets.FirstOrDefault(s => s.Name == name); }
        }
    }
}