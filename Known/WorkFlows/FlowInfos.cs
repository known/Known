using System.ComponentModel;

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
    [DisplayName("ID")]
    public string Id { get; set; }
    [DisplayName("名称")]
    public string Name { get; set; }
    [DisplayName("类型")]
    public string Type { get; set; }
    [DisplayName("操作用户")]
    public string User { get; set; }
    [DisplayName("操作角色")]
    public string Role { get; set; }
    [DisplayName("通过状态")]
    public string Pass { get; set; }
    [DisplayName("退回状态")]
    public string Fail { get; set; }
}