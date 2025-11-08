namespace Known.Sample.Components;

[Role("测试组件")]
public class TestRole : BaseTable<WeatherForecast>
{
    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();

        Table.Toolbar.AddAction<TestRole>(nameof(AddTE));
        Table.Toolbar.AddAction<TestRole>(nameof(Save));
    }

    [Action(Name = "添加TE", Icon = "plus")]
    public void AddTE() { }

    [Action] public void Save() { }
}