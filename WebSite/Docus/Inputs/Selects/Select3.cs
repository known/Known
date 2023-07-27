namespace WebSite.Docus.Inputs.Selects;

class Select3 : BaseComponent
{
    private Select? select;

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Component<FieldControl>()
               .Set(c => c.OnVisibleChanged, OnVisibleChanged)
               .Set(c => c.OnEnabledChanged, OnEnabledChanged)
               .Set(c => c.SetValue, SetValue)
               .Set(c => c.GetValue, GetValue)
               .Build();

        builder.Field<Select>("英雄：", "Select")
               .Set(f => f.Codes, "孙膑,后羿,妲己")
               .Build(value => select = value);
    }

    private void OnVisibleChanged(bool value) => select?.SetVisible(value);
    private void OnEnabledChanged(bool value) => select?.SetEnabled(value);
    private void SetValue() => select?.SetValue("孙膑");
    private string? GetValue() => select?.Value;
}