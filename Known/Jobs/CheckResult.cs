using System.Collections.Generic;

namespace Known.Jobs
{
    public class CheckResult
    {
        public bool IsPass { get; set; }
        public int TimerInterval { get; set; }
        public string TimeFormat { get; set; }
        public List<string> TimeValues { get; set; }
        public IJob Instance { get; set; }
        public string ErrorMessage { get; set; }
    }
}