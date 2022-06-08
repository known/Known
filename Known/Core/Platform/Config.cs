namespace Known.Core
{
    partial class PlatformService
    {
        public string GetConfig(string key)
        {
            return GetConfig(App.AppId, key);
        }

        public string GetConfig(string appId, string key)
        {
            return Repository.GetConfig(Database, appId, key);
        }

        public void SaveConfig(string key, string value)
        {
            SaveConfig(App.AppId, key, value);
        }

        public void SaveConfig(string appId, string key, string value)
        {
            Repository.SaveConfig(Database, appId, key, value);
        }

        public T GetConfig<T>(string key)
        {
            var json = GetConfig(key);
            return Utils.FromJson<T>(json);
        }

        public void SaveConfig(string key, object value)
        {
            var json = Utils.ToJson(value);
            SaveConfig(key, json);
        }
    }

    partial interface IPlatformRepository
    {
        string GetConfig(Database db, string appId, string key);
        void DeleteConfig(Database db, string appId, string key);
        void SaveConfig(Database db, string appId, string key, string value);
    }

    partial class PlatformRepository
    {
        public string GetConfig(Database db, string appId, string key)
        {
            var sql = "select ConfigValue from SysConfig where AppId=@appId and ConfigKey=@key";
            return db.Scalar<string>(sql, new { appId, key });
        }

        public void DeleteConfig(Database db, string appId, string key)
        {
            var sql = "delete from SysConfig where AppId=@appId and ConfigKey=@key";
            db.Execute(sql, new { appId, key });
        }

        public void SaveConfig(Database db, string appId, string key, string value)
        {
            var sql = "select count(*) from SysConfig where AppId=@appId and ConfigKey=@key";
            if (db.Scalar<int>(sql, new { appId, key }) > 0)
            {
                sql = "update SysConfig set ConfigValue=@value where AppId=@appId and ConfigKey=@key";
                db.Execute(sql, new { value, appId, key });
            }
            else
            {
                sql = "insert into SysConfig(AppId,ConfigKey,ConfigValue) values(@appId,@key,@value)";
                db.Execute(sql, new { appId, key, value });
            }
        }
    }
}
