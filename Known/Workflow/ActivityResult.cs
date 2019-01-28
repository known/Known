namespace Known.Workflow
{
    public class ActivityResult
    {
        public bool IsPass { get; set; }
        public string Message { get; set; }
        public FlowInstance Flow { get; set; }
    }
}