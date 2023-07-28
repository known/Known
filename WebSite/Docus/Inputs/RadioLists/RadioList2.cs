namespace WebSite.Docus.Inputs.RadioLists;

class RadioList2 : BaseComponent
{
    private RadioList? radioList;

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Component<FieldControl>()
               .Set(c => c.OnVisibleChanged, OnVisibleChanged)
               .Set(c => c.OnEnabledChanged, OnEnabledChanged)
               .Set(c => c.OnReadOnlyChanged, OnReadOnlyChanged)
               .Set(c => c.SetValue, SetValue)
               .Set(c => c.GetValue, GetValue)
               .Build();

        builder.Field<RadioList>("RadioList")
               .Set(f => f.Codes, "孙膑,后羿,妲己")
               .Build(value => radioList = value);
    }

    private void OnVisibleChanged(bool value) => radioList?.SetVisible(value);
    private void OnEnabledChanged(bool value) => radioList?.SetEnabled(value);
    private void OnReadOnlyChanged(bool value) => radioList?.SetReadOnly(value);
    private void SetValue() => radioList?.SetValue("孙膑");
    private string? GetValue() => radioList?.Value;
}