using System;
using System.Data;
using Known.Extensions;

namespace Known.Tests.Core.Extensions
{
    public class CompressExtensionTest
    {
        public static void TestCompress()
        {
            var str = new String('T', 5000);
            var length1 = str.ToBytes().Length;
            var length2 = str.Compress().Length;
            Assert.IsEqual(length1 > length2, true);

            var set = new DataSet();
            set.Tables.Add(new DataTable());
            Assert.IsEqual(set.Compress().Length, 242);
        }

        public static void TestDecompress()
        {
            var bytes1 = "test".Compress();
            Assert.IsEqual(bytes1.Decompress<string>(), "test");

            var set = new DataSet();
            set.Tables.Add(new DataTable());
            var bytes = set.Compress();
            Assert.IsEqual(bytes.Decompress().Tables.Count, 1);
        }
    }
}
