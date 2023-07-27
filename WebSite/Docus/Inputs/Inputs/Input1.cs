namespace WebSite.Docus.Inputs.Inputs;

class Input1 : BaseComponent
{
    private Input? input;

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Component<FieldControl>()
               .Set(c => c.OnVisibleChanged, OnVisibleChanged)
               .Set(c => c.OnEnabledChanged, OnEnabledChanged)
               .Set(c => c.SetValue, SetValue)
               .Set(c => c.GetValue, GetValue)
               .Build();

        builder.Field<Input>("日期", "Date").Value("2023-01-01").Build(value => input = value);
    }

    private void OnVisibleChanged(bool value) => input?.SetVisible(value);
    private void OnEnabledChanged(bool value) => input?.SetEnabled(value);
    private void SetValue() => input?.SetValue($"{DateTime.Now:yyyy-MM-dd}");
    private string? GetValue() => input?.Value;
}