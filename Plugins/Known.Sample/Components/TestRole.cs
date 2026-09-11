namespace Known.Sample.Components;

[Role("测试组件")]
public class TestRole : BaseTable<Weather_Forecast>
{
    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        Table.TopTemplate = b => b.Div("kui-bold", "测试顶部组件");
    }

    [Action(Name = "添加TE", Icon = "plus")]
    public void AddTE() { }

    [Action] public void Save() { }
}