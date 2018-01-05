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

        public static void TestGetDefaultDatabase()
        {
            var database = Config.GetDatabase();
            Assert.IsEqual(database.Type, Data.DatabaseType.SqlServer);
            Assert.IsEqual(database.ConnectionString, @"Data Source=.\SQLEXPRESS;Database=SnsLite;User Id=sa;Password=123;");
        }

        public static void TestGetDatabase()
        {
            var database = Config.GetDatabase("Default");
            Assert.IsEqual(database.Type, Data.DatabaseType.SqlServer);
            Assert.IsEqual(database.ConnectionString, @"Data Source=.\SQLEXPRESS;Database=SnsLite;User Id=sa;Password=123;");
        }
    }
}
