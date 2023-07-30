namespace WebSite.Docus.Inputs.RichTexts;

class RichText3 : BaseComponent
{
    private RichText? richText;

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Component<FieldControl>()
               .Set(c => c.OnVisibleChanged, OnVisibleChanged)
               .Set(c => c.OnEnabledChanged, OnEnabledChanged)
               .Set(c => c.OnReadOnlyChanged, OnReadOnlyChanged)
               .Set(c => c.SetValue, SetValue)
               .Set(c => c.GetValue, GetValue)
               .Build();

        builder.Field<RichText>("内容：", "RichText3").Build(value => richText = value);
    }

    private void OnVisibleChanged(bool value) => richText?.SetVisible(value);
    private void OnEnabledChanged(bool value) => richText?.SetEnabled(value);
    private void OnReadOnlyChanged(bool value) => richText?.SetReadOnly(value);
    private void SetValue() => richText?.SetValue("<h1>孙膑</h1>");
    private string? GetValue() => richText?.Value;
}