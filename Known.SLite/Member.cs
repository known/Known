using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Known.SLite
{
    public class Member : Entity<Member>
    {
        public Member()
        {
            Status = 0;
        }

        public string RealName { get; set; }
        public string Mobile { get; set; }
        public string Password { get; set; }
        public string RegisterIP { get; set; }
        public int Status { get; set; }

        public override List<string> Validate()
        {
            var list = new List<string>();
            if (string.IsNullOrEmpty(RealName))
                list.Add("真实姓名不能为空！");
            if (string.IsNullOrEmpty(Mobile))
                list.Add("手机号码不能为空！");
            return list;
        }
    }
}
