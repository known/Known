namespace Known.WorkFlows;

public class FlowFormInfo
{
    public string BizId { get; set; }
    public string BizStatus { get; set; }
    public string User { get; set; }
    public string UserRole { get; set; }
    public string Note { get; set; }
    public string FlowStatus { get; set; }
    public object Model { get; set; }
}

public class FlowBizInfo
{
    public string FlowCode { get; set; }
    public string FlowName { get; set; }
    public string BizId { get; set; }
    public string BizName { get; set; }
    public string BizUrl { get; set; }
    public string BizStatus { get; set; }
    public UserInfo CurrUser { get; set; }
}

public class FlowInfo
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int? Width { get; set; }
    public int? Height { get; set; }
    public List<FlowStepInfo> Steps { get; set; } = [];
}

public class FlowStepInfo
{
    [DisplayName("Id")] public string Id { get; set; }
    [DisplayName("Name")] public string Name { get; set; }
    [DisplayName("User")] public string User { get; set; }
    [DisplayName("Role")] public string Role { get; set; }
    [DisplayName("Pass")] public string Pass { get; set; }
    [DisplayName("Fail")] public string Fail { get; set; }
}