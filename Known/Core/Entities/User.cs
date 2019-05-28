using System;
using Known.Mapping;

namespace Known.Core.Entities
{
    public class User : EntityBase
    {
        public string AppId { get; set; }
        public string CompanyId { get; set; }
        public string DepartmentId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string Phone { get; set; }
        public string Token { get; set; }
        public DateTime? FirstLoginTime { get; set; }
        public DateTime? LastLoginTime { get; set; }
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
                .IsStringColumn("token", "票据", 1, 50);

            this.Property(p => p.FirstLoginTime)
                .IsDateTimeColumn("first_login_time", "首次登录时间");

            this.Property(p => p.LastLoginTime)
                .IsDateTimeColumn("last_login_time", "最后登录时间");

            this.Property(p => p.SettingsData)
                .IsStringColumn("settings_data", "设定数据");
        }
    }
}
