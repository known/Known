using System;

namespace Known.Entities
{
    public class SysUser : EntityBase
    {
        [Column("组织编码", "", true, "1", "50")]
        public string OrgNo { get; set; }

        [Column("用户名", "", true, "1", "50")]
        public string UserName { get; set; }

        [Column("密码", "", true, "1", "50")]
        public string Password { get; set; }

        [Column("姓名", "", true, "1", "50")]
        public string Name { get; set; }

        [Column("英文名", "", false, "1", "50")]
        public string EnglishName { get; set; }

        [Column("性别", "", true, "1", "50")]
        public string Gender { get; set; }

        [Column("座机", "", false, "1", "50")]
        public string Phone { get; set; }

        [Column("手机", "", false, "1", "50")]
        public string Mobile { get; set; }

        [Column("邮箱", "", false, "1", "50")]
        public string Email { get; set; }

        [Column("状态", "", true)]
        public int Enabled { get; set; }

        [Column("备注", "", false, "1", "500")]
        public string Note { get; set; }

        [Column("角色", "", false, "1", "500")]
        public string Role { get; set; }

        [Column("首次登录时间", "", false)]
        public DateTime? FirstLoginTime { get; set; }

        [Column("首次登录IP", "", false, "1", "50")]
        public string FirstLoginIP { get; set; }

        [Column("最近登录时间", "", false)]
        public DateTime? LastLoginTime { get; set; }

        [Column("最近登录IP", "", false, "1", "50")]
        public string LastLoginIP { get; set; }
    }
}