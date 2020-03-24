using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Known.Jobs
{
    public enum JobStatus
    {
        [Description("已停用")]
        Disable = 0,
        [Description("运行正常")]
        Normal = 1,
        [Description("运行异常")]
        Abnormal = 2,
        [Description("运行中")]
        Running = 3
    }

    public class JobInfo
    {
        public string Id { get; set; }
        public string Server { get; set; }
        public string Name { get; set; }
        public string ExecuteTarget { get; set; }
        public string ExecuteInterval { get; set; }
        public JobStatus Status { get; set; }
        public int? SuccessCount { get; set; }
        public int? FailCount { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string Message { get; set; }
        public string Enabled { get; set; }
        public int RunCount { get; set; }
        public bool IsRestart { get; set; }
        public Dictionary<string, object> Config { get; set; }
    }
}