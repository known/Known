using System.ComponentModel;

namespace Known.Workflow
{
    public enum Action
    {
        [Description("未执行")]
        None = 0,
        [Description("通过")]
        Pass = 1,
        [Description("退回")]
        Return = 2,
        [Description("撤销")]
        Revoke = 3,
        [Description("终止")]
        Stop = 4
    }

    public class ActivityContext
    {
        public Action Action { get; set; }
        public ProcessInstance Instance { get; set; }
    }
}