using System;
using Known.Extensions;

namespace Known.Tests.Core.Extensions
{
    public class DateTimeExtensionTest
    {
        public static void TestDateTimeToTimestamp()
        {
            var date = new DateTime(2017, 10, 1, 10, 1, 1);
            TestAssert.AreEqual(date.ToTimestamp(), 1506823261000);
        }

        public static void TestStringToDateTime()
        {
            TestAssert.AreEqual("2017-10-01".ToDateTime("yyyy-MM-dd"), new DateTime(2017, 10, 1));
        }

        public static void TestDateTimeToString()
        {
            var date = new DateTime(2017, 10, 1, 10, 1, 1);
            TestAssert.AreEqual(date.ToString("yyyy-MM-dd HH:mm:ss"), "2017-10-01 10:01:01");
        }
    }
}
