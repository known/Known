namespace Sample.Components;

[Role("测试组件")]
public class TestRole : BaseTable<WeatherForecast>
{
    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();

        if (Context.HasButton<TestRole>(nameof(AddTE)))
            Table.Toolbar.AddAction(nameof(AddTE));
        if (Context.HasButton<TestRole>(nameof(Save)))
            Table.Toolbar.AddAction(nameof(Save));
    }

    [Action(Name = "添加TE", Icon = "plus")]
    public void AddTE() { }

    [Action] public void Save() { }
}