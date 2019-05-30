using System;
using System.Data;
using Known.Extensions;

namespace Known.Tests.Extensions
{
    public class CompressExtensionTest
    {
        public static void Compress()
        {
            var str = new String('T', 5000);
            var length1 = str.ToBytes().Length;
            var length2 = str.Compress().Length;
            TestAssert.AreEqual(length1 > length2, true);

            var set = new DataSet();
            set.Tables.Add(new DataTable());
            TestAssert.AreEqual(set.Compress().Length, 242);
        }

        public static void Decompress()
        {
            var bytes1 = "test".Compress();
            TestAssert.AreEqual(bytes1.Decompress<string>(), "test");

            var set = new DataSet();
            set.Tables.Add(new DataTable());
            var bytes = set.Compress();
            TestAssert.AreEqual(bytes.Decompress().Tables.Count, 1);
        }
    }
}
