using System;
using Known.Core;

namespace Known.Workflow
{
    public class Activity
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Note { get; set; }
        public int Order { get; set; }
        public UserInfo ExecuteBy { get; set; }
        public DateTime ExecuteTime { get; set; }
    }
}
