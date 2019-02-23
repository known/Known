using System;
using System.Collections.Generic;
using System.Data;
using Known.Data;
using Known.Extensions;

namespace Known.Platform
{
    public interface IPlatformRepository
    {
        string GetApiBaseUrl(string apiId);
        Dictionary<string, object> GetCodes(string appId);
        Module GetModule(string id);
        List<Module> GetModules(string appId);
        List<Module> GetUserModules(string appId, string userName);
        User GetUser(string userName);
        User GetUserByToken(string token);
        void SaveUser(User user);
    }

    class PlatformRepository : IPlatformRepository
    {
        private readonly Database database;

        public PlatformRepository()
        {
            database = new Database();
        }

        public string GetApiBaseUrl(string apiId)
        {
            var apiUrl = string.Empty;
            if (apiId == "plt")
                apiUrl = "http://localhost:8089";

            return apiUrl;
        }

        public Dictionary<string, object> GetCodes(string appId)
        {
            return new Dictionary<string, object>();
        }

        public Module GetModule(string id)
        {
            var sql = "select * from t_plt_modules where id=@id";
            var row = database.QueryRow(sql, new { id });
            if (row == null)
                return null;

            return GetModule(row);
        }

        public List<Module> GetModules(string appId)
        {
            var sql = "select * from t_plt_modules where app_id=@appId";
            var data = database.QueryTable(sql, new { appId });
            if (data == null || data.Rows.Count == 0)
                return null;

            var modules = new List<Module>();
            foreach (DataRow row in data.Rows)
            {
                modules.Add(GetModule(row));
            }

            return modules;
        }

        public List<Module> GetUserModules(string appId, string userName)
        {
            return GetModules(appId);
        }

        public User GetUser(string userName)
        {
            var sql = "select * from t_plt_users where user_name=@userName";
            var row = database.QueryRow(sql, new { userName });
            if (row == null)
                return null;

            return GetUser(row);
        }

        public User GetUserByToken(string token)
        {
            var sql = "select * from t_plt_users where token=@token";
            var row = database.QueryRow(sql, new { token });
            if (row == null)
                return null;

            return GetUser(row);
        }

        public void SaveUser(User user)
        {
            if (user == null)
                return;

            var sql = @"
update t_plt_users set
  token=@Token
 ,first_login_time=@FirstLoginTime
 ,last_login_time=@LastLoginTime 
where id=@Id";
            database.Execute(sql, user);
        }

        private static Module GetModule(DataRow row)
        {
            return new Module
            {
                Id = row.Get<string>("id"),
                AppId = row.Get<string>("app_id"),
                ParentId = row.Get<string>("parent_id"),
                Code = row.Get<string>("code"),
                Name = row.Get<string>("name"),
                Description = row.Get<string>("description"),
                ViewType = row.Get<ViewType>("view_type"),
                Url = row.Get<string>("url"),
                Icon = row.Get<string>("icon"),
                Sort = row.Get<int>("sort"),
                Enabled = row.Get<bool>("enabled"),
                ButtonData = row.Get<string>("button_data"),
                FieldData = row.Get<string>("field_data"),
                Extension = row.Get<string>("extension")
            };
        }

        private static User GetUser(DataRow row)
        {
            return new User
            {
                Id = row.Get<string>("id"),
                AppId = row.Get<string>("app_id"),
                CompanyId = row.Get<string>("company_id"),
                DepartmentId = row.Get<string>("department_id"),
                UserName = row.Get<string>("user_name"),
                Password = row.Get<string>("password"),
                Name = row.Get<string>("name"),
                Email = row.Get<string>("email"),
                Mobile = row.Get<string>("mobile"),
                Phone = row.Get<string>("phone"),
                Token = row.Get<string>("token"),
                FirstLoginTime = row.Get<DateTime?>("first_login_time"),
                LastLoginTime = row.Get<DateTime?>("last_login_time"),
                SettingsData = row.Get<string>("settings_data")
            };
        }
    }
}
