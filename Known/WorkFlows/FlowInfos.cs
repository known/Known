using System.ComponentModel.DataAnnotations;

namespace Known.WorkFlows;

public class FlowFormInfo
{
    public string BizId { get; set; }
    public string BizStatus { get; set; }
    public string User { get; set; }
    public string UserRole { get; set; }
    [MaxLength(500)]
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
    public List<FlowStepInfo> Steps { get; set; }
}

public class FlowStepInfo
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Flag { get; set; }
    public bool Round { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public string Arrow { get; set; }
}