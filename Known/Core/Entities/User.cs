using System;
using Known.Mapping;

namespace Known.Core.Entities
{
    /// <summary>
    /// 应用程序用户实体类。
    /// </summary>
    public class User : EntityBase
    {
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
    }

    class UserMapper : EntityMapper<User>
    {
        public UserMapper() :
            base("t_plt_users", "系统用户")
        {
            this.Property(p => p.AppId)
                .IsStringColumn("app_id", "APPID", 1, 50, true);

            this.Property(p => p.CompanyId)
                .IsStringColumn("company_id", "公司ID", 1, 50, true);

            this.Property(p => p.DepartmentId)
                .IsStringColumn("department_id", "部门ID", 1, 50, true);

            this.Property(p => p.UserName)
                .IsStringColumn("user_name", "用户名", 1, 50, true);

            this.Property(p => p.Password)
                .IsStringColumn("password", "密码", 1, 50, true);

            this.Property(p => p.Name)
                .IsStringColumn("name", "姓名", 1, 50);

            this.Property(p => p.Email)
                .IsStringColumn("email", "邮箱", 1, 50);

            this.Property(p => p.Mobile)
                .IsStringColumn("mobile", "手机", 1, 50);

            this.Property(p => p.Phone)
                .IsStringColumn("phone", "座机", 1, 50);

            this.Property(p => p.Token)
                .IsStringColumn("token", "身份认证票据", 1, 50);

            this.Property(p => p.FirstLoginTime)
                .IsDateTimeColumn("first_login_time", "首次登录时间");

            this.Property(p => p.LastLoginTime)
                .IsDateTimeColumn("last_login_time", "最后登录时间");

            this.Property(p => p.SettingsData)
                .IsStringColumn("settings_data", "设定数据");
        }
    }
}
