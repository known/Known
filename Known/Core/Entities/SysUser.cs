using System;

namespace Known.Core.Entities
{
    public class SysUser : EntityBase
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string EnglishName { get; set; }
        public string Gender { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public int Enabled { get; set; }
        public DateTime? FirstLoginTime { get; set; }
        public string FirstLoginIP { get; set; }
        public DateTime? LastLoginTime { get; set; }
        public string LastLoginIP { get; set; }
    }
}
