using Known.Cells;

namespace Known.Tests.Cells
{
    public class ExcelTest
    {
        public static void Constructor1()
        {
            var excel = new Excel();
            TestAssert.IsNotNull(excel);
            TestAssert.AreEqual(excel.Sheets.Count, 0);
        }
    }
}
