namespace Known.Sample.Components;

[Role("测试组件")]
public class TestRole : BaseTable<Weather_Forecast>
{
    [Action(Name = "添加TE", Icon = "plus")]
    public void AddTE() { }

    [Action] public void Save() { }
}