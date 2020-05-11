using System;

namespace Known.Core.Entities
{
    public class SysUser : EntityBase
    {
        [Column("用户名", "", true, "1", "50")]
        public string UserName { get; set; }

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

        public DateTime? FirstLoginTime { get; set; }

        public string FirstLoginIP { get; set; }

        public DateTime? LastLoginTime { get; set; }

        public string LastLoginIP { get; set; }
    }
}
