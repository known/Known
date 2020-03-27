using System;
using Known.Extensions;

namespace Known.Tests.Extensions
{
    public class DateTimeExtensionTest
    {
        public static void DateTimeToTimestamp()
        {
            var date = new DateTime(2017, 10, 1, 10, 1, 1);
            TestAssert.AreEqual(date.ToTimestamp(), 1506823261000);
        }

        public static void StringToDateTime()
        {
            TestAssert.AreEqual("2017-10-01".ToDateTime("yyyy-MM-dd"), new DateTime(2017, 10, 1));
        }

        public static void DateTimeToString()
        {
            var date = new DateTime(2017, 10, 1, 10, 1, 1);
            TestAssert.AreEqual(date.ToString("yyyy-MM-dd HH:mm:ss"), "2017-10-01 10:01:01");
        }
    }
}
