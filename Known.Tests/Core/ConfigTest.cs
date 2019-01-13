namespace Known.Tests.Core
{
    public class ConfigTest
    {
        public static void TestAppSetting()
        {
            TestAssert.AreEqual(Config.AppSetting("test"), "测试");
            TestAssert.AreEqual(Config.AppSetting<int>("vint"), 1);
            TestAssert.AreEqual(Config.AppSetting<bool>("vbool"), true);
            TestAssert.AreEqual(Config.AppSetting("none", "test"), "test");
            TestAssert.AreEqual(Config.AppSetting<int>("int", 1), 1);
        }
    }
}
