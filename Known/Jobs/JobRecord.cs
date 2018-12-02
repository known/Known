using System;
using System.ComponentModel;

namespace Known.Jobs
{
    public enum RecordStatus
    {
        [Description("执行成功")]
        Success = 1,
        [Description("执行失败")]
        Failure = 2
    }

    public class JobRecord
    {
        public string JobId { get; set; }
        public string Server { get; set; }
        public string Name { get; set; }
        public string ExecuteTarget { get; set; }
        public string ExecuteInterval { get; set; }
        public RecordStatus Status { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string Message { get; set; }
        public string LogInfo { get; set; }
    }
}