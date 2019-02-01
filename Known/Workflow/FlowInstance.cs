using System.ComponentModel;

namespace Known.Workflow
{
    public enum FlowStatus
    {
        [Description("流转中")]
        Running = 1,
        [Description("已完成")]
        Closed = 2,
        [Description("已终止")]
        Stopped = 3
    }

    public class FlowInstance
    {
        public string Id { get; set; }
        public string BillId { get; set; }
        public string BillStatus { get; set; }
        public Flow Flow { get; set; }
        public FlowStatus Status { get; set; }
        public Activity Activity { get; set; }
        public Activity PrevActivity { get; set; }
        public Activity NextActivity { get; set; }
    }
}
