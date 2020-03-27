using Known.Extensions;

namespace Known.Tests.Extensions
{
    public class EnumExtensionTest
    {
        public static void GetDescription()
        {
            TestAssert.AreEqual(TestEnum.Enum1.GetDescription(), "枚举1");
            TestAssert.AreEqual(TestEnum.Enum2.GetDescription(), "枚举2");
        }
    }
}
