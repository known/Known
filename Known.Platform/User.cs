using System;
using System.Collections.Generic;
using Known.Mapping;

namespace Known.Platform
{
    [Table("t_plt_users", "系统用户")]
    public class User : BaseEntity
    {
        [StringColumn("company_id", "公司ID", 1, 50, true)]
        public string CompanyId { get; set; }

        [StringColumn("department_id", "部门ID", 1, 50, true)]
        public string DepartmentId { get; set; }

        [StringColumn("user_name", "用户名", 1, 50, true)]
        public string UserName { get; set; }

        [StringColumn("password", "密码", 1, 50, true)]
        public string Password { get; set; }

        [StringColumn("name", "姓名", 1, 50)]
        public string Name { get; set; }

        [StringColumn("email", "邮箱", 1, 50)]
        public string Email { get; set; }

        [StringColumn("mobile", "手机", 1, 50)]
        public string Mobile { get; set; }

        [StringColumn("phone", "座机", 1, 50)]
        public string Phone { get; set; }

        [StringColumn("token", "票据", 1, 50)]
        public string Token { get; set; }

        [DateTimeColumn("first_login_time", "首次登录时间")]
        public DateTime? FirstLoginTime { get; set; }

        [DateTimeColumn("last_login_time", "最后登录时间")]
        public DateTime? LastLoginTime { get; set; }

        [StringColumn("settings_data", "设定数据")]
        public string SettingsData { get; set; }

        public virtual Company Company { get; set; }
        public virtual Department Department { get; set; }
        public virtual User Manager { get; set; }
        public virtual List<User> Proxys { get; set; }
        public virtual List<Application> Applications { get; set; }
    }
}
