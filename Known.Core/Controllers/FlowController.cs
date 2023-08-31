namespace Known.Core.Controllers;

[Route("[controller]")]
public class FlowController : BaseController
{
    private FlowService Service => new(Context);

    [HttpGet("[action]")]
    public List<SysFlow> GetFlowTodos() => Service.GetFlowTodos();

    [HttpGet("[action]")]
    public List<SysFlowLog> GetFlowLogs([FromQuery] string bizId) => Service.GetFlowLogs(bizId);

    [HttpPost("[action]")]
    public Result SubmitFlow([FromBody] FlowFormInfo info) => Service.SubmitFlow(info);

    [HttpPost("[action]")]
    public Result RevokeFlow([FromBody] FlowFormInfo info) => Service.RevokeFlow(info);

    [HttpPost("[action]")]
    public Result AssignFlow([FromBody] FlowFormInfo info) => Service.AssignFlow(info);

    [HttpPost("[action]")]
    public Result VerifyFlow([FromBody] FlowFormInfo info) => Service.VerifyFlow(info);

    [HttpPost("[action]")]
    public Result RepeatFlow([FromBody] FlowFormInfo info) => Service.RepeatFlow(info);

    [HttpPost("[action]")]
    public Result StopFlow([FromBody] FlowFormInfo info) => Service.StopFlow(info);
}