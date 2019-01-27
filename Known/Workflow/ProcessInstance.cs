using System.ComponentModel;

namespace Known.Workflow
{
    public enum ProcessStatus
    {
        [Description("流转中")]
        Running = 1,
        [Description("已完成")]
        Closed = 2,
        [Description("已终止")]
        Stopped = 3
    }

    public class ProcessInstance
    {
        public string Id { get; set; }
        public string ProcessId { get; set; }
        public string BillId { get; set; }
        public string BillStatus { get; set; }
        public ProcessStatus Status { get; set; }
        public Activity Activity { get; set; }
        public Activity PrevActivity { get; set; }
        public Activity NextActivity { get; set; }
    }
}
