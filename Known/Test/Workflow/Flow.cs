using System.Collections.Generic;

namespace Known.Workflow
{
    public class Flow
    {
        public string Id { get; set; }
        public string Category { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Note { get; set; }
        public List<Activity> Activities { get; set; }
    }
}
