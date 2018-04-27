using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Known.Extensions;

namespace Known.Tests.KnownTests.Extensions
{
    public class EnumExtensionTest
    {
        public static void TestEnumGetDescription()
        {
            Assert.IsEqual(TestEnum.Enum1.GetDescription(), "枚举1");
            Assert.IsEqual(TestEnum.Enum2.GetDescription(), "枚举2");
        }
    }
}
