namespace Known.Tests.KnownTests
{
    public class ConfigTest
    {
        public static void TestAppSetting()
        {
            Assert.IsEqual(Config.AppSetting("test"), "测试");
            Assert.IsEqual(Config.AppSetting<int>("vint"), 1);
            Assert.IsEqual(Config.AppSetting<bool>("vbool"), true);
        }
    }
}
