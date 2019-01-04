namespace Known.Tests.Core
{
    public class ConfigTest
    {
        public static void TestAppSetting()
        {
            Assert.AreEqual(Config.AppSetting("test"), "测试");
            Assert.AreEqual(Config.AppSetting<int>("vint"), 1);
            Assert.AreEqual(Config.AppSetting<bool>("vbool"), true);
            Assert.AreEqual(Config.AppSetting("none", "test"), "test");
            Assert.AreEqual(Config.AppSetting<int>("int", 1), 1);
        }
    }
}
