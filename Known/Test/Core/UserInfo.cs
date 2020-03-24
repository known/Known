using System;
using System.Collections.Generic;

namespace Known.Core
{
    /// <summary>
    /// 用户信息类。
    /// </summary>
    public class UserInfo
    {
        /// <summary>
        /// 取得或设置用户ID。
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 取得或设置应用程序ID。
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// 取得或设置公司ID。
        /// </summary>
        public string CompanyId { get; set; }

        /// <summary>
        /// 取得或设置部门ID。
        /// </summary>
        public string DepartmentId { get; set; }

        /// <summary>
        /// 取得或设置用户名。
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 取得或设置密码。
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 取得或设置姓名。
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 取得或设置邮箱。
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 取得或设置手机。
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 取得或设置座机。
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 取得或设置身份认证票据。
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// 取得或设置首次登录时间。
        /// </summary>
        public DateTime? FirstLoginTime { get; set; }

        /// <summary>
        /// 取得或设置最后登录时间。
        /// </summary>
        public DateTime? LastLoginTime { get; set; }

        /// <summary>
        /// 取得或设置设定数据。
        /// </summary>
        public string SettingsData { get; set; }

        /// <summary>
        /// 取得或设置用户所属公司对象。
        /// </summary>
        public CompanyInfo Company { get; set; }

        /// <summary>
        /// 取得或设置用户所属部门对象。
        /// </summary>
        public DepartmentInfo Department { get; set; }

        /// <summary>
        /// 取得或设置用户所属管理者对象。
        /// </summary>
        public UserInfo Manager { get; set; }

        /// <summary>
        /// 取得或设置用户所有代理人列表。
        /// </summary>
        public List<UserInfo> Proxys { get; set; }

        /// <summary>
        /// 取得或设置用户所有应用程序列表。
        /// </summary>
        public List<AppInfo> Applications { get; set; }
    }
}