namespace WebSite.Docus.Inputs.Texts;

class Text3 : BaseComponent
{
    private Text? text;

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Component<FieldControl>()
               .Set(c => c.OnVisibleChanged, OnVisibleChanged)
               .Set(c => c.OnEnabledChanged, OnEnabledChanged)
               .Set(c => c.SetValue, SetValue)
               .Set(c => c.GetValue, GetValue)
               .Build();

        builder.Field<Text>("示例：", "Text").Build(value => text = value);
    }

    private void OnVisibleChanged(bool value) => text?.SetVisible(value);
    private void OnEnabledChanged(bool value) => text?.SetEnabled(value);
    private void SetValue() => text?.SetValue("孙膑");
    private string? GetValue() => text?.Value;
}