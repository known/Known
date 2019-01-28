using System;

namespace Known.Workflow
{
    public class FlowLog
    {
        public string Id { get; set; }
        public string BillId { get; set; }
        public string Activity { get; set; }
        public string ExecuteBy { get; set; }
        public DateTime ExecuteTime { get; set; }
        public string Result { get; set; }
        public string Note { get; set; }
    }
}
