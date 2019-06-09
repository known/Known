using System;
using System.Collections.Generic;
using System.Data;
using Known.Data;
using Known.Extensions;
using Known.Web;

namespace Known.Core
{
    /// <summary>
    /// 平台数据仓库接口。
    /// </summary>
    public interface IPlatformRepository
    {
        /// <summary>
        /// 获取指定应用程序的 API 接口根地址。
        /// </summary>
        /// <param name="apiId">应用程序 ID。</param>
        /// <returns>API 接口根地址。</returns>
        string GetApiBaseUrl(string apiId);

        /// <summary>
        /// 获取指定应用程序的系统代码列表。
        /// </summary>
        /// <param name="appId">应用程序 ID。</param>
        /// <returns>系统代码列表。</returns>
        Dictionary<string, object> GetCodes(string appId);

        /// <summary>
        /// 获取模块信息对象。
        /// </summary>
        /// <param name="id">模块 ID。</param>
        /// <returns>模块信息对象。</returns>
        Module GetModule(string id);

        /// <summary>
        /// 获取指定应用程序的模块信息对象列表。
        /// </summary>
        /// <param name="appId">应用程序 ID。</param>
        /// <returns>模块信息对象列表。</returns>
        List<Module> GetModules(string appId);

        /// <summary>
        /// 获取指定应用程序和登录用户的模块信息对象列表。
        /// </summary>
        /// <param name="appId">应用程序 ID。</param>
        /// <param name="userName">登录用户名。</param>
        /// <returns>模块信息对象列表。</returns>
        List<Module> GetUserModules(string appId, string userName);

        /// <summary>
        /// 获取指定用户名的用户信息对象。
        /// </summary>
        /// <param name="userName">登录用户名。</param>
        /// <returns>用户信息对象。</returns>
        User GetUser(string userName);

        /// <summary>
        /// 获取指定用户身份票据的用户信息对象。
        /// </summary>
        /// <param name="token">用户身份票据。</param>
        /// <returns>用户信息对象。</returns>
        User GetUserByToken(string token);

        /// <summary>
        /// 保存用户信息对象。
        /// </summary>
        /// <param name="user">用户信息对象。</param>
        void SaveUser(User user);
    }

    class ApiPlatformRepository : IPlatformRepository
    {
        private static readonly Dictionary<string, string> apiBaseUrls = new Dictionary<string, string>();
        private readonly ApiClient client;

        public ApiPlatformRepository(string baseUrl)
        {
            client = new ApiClient(baseUrl);
        }

        public string GetApiBaseUrl(string apiId)
        {
            if (string.IsNullOrWhiteSpace(apiId))
                return null;

            if (!apiBaseUrls.ContainsKey(apiId))
            {
                apiBaseUrls[apiId] = client.Get<string>("/api/App/GetApiUrl", new { apiId });
            }

            return apiBaseUrls[apiId];
        }

        public Dictionary<string, object> GetCodes(string appId)
        {
            return client.Get<Dictionary<string, object>>("/api/App/GetCodes", new { appId });
        }

        public Module GetModule(string id)
        {
            return client.Get<Module>("/api/Module/GetModule", new { id });
        }

        public List<Module> GetModules(string appId)
        {
            return client.Get<List<Module>>("/api/Module/GetModules", new { appId });
        }

        public List<Module> GetUserModules(string appId, string userName)
        {
            return client.Get<List<Module>>("/api/User/GetUserModules", new { appId, userName });
        }

        public User GetUser(string userName)
        {
            return client.Get<User>("/api/User/GetUser", new { userName });
        }

        public User GetUserByToken(string token)
        {
            return client.Get<User>("/api/User/GetUserByToken", new { token });
        }

        public void SaveUser(User user)
        {
            client.Post("/api/User/SaveUser", user);
        }
    }

    class PlatformRepository : IPlatformRepository
    {
        private readonly Database database;

        public PlatformRepository(Database database)
        {
            this.database = database;
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
            var sql = "select * from t_plt_modules where app_id=@appId and enabled=1";
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
