using Known.Cells;

namespace Known.Tests.Core.Cells
{
    public class ExcelTest
    {
        public static void Constructor1()
        {
            var provider = new AsposeExcel();
            var excel = new Excel(provider);
            TestAssert.IsNotNull(excel);
            TestAssert.AreEqual(excel.Sheets.Count, 0);
        }
    }
}
