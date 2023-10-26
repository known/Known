namespace WebSite.Docus.Inputs.CheckLists;

class CheckList2 : BaseComponent
{
    private KCheckList? checkList;

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Component<FieldControl>()
               .Set(c => c.OnVisibleChanged, OnVisibleChanged)
               .Set(c => c.OnEnabledChanged, OnEnabledChanged)
               .Set(c => c.OnReadOnlyChanged, OnReadOnlyChanged)
               .Set(c => c.SetValue, SetValue)
               .Set(c => c.GetValue, GetValue)
               .Build();

        builder.Field<KCheckList>("英雄：", "CheckList")
               .Set(f => f.Codes, "孙膑,后羿,妲己")
               .Build(value => checkList = value);
    }

    private void OnVisibleChanged(bool value) => checkList?.SetVisible(value);
    private void OnEnabledChanged(bool value) => checkList?.SetEnabled(value);
    private void OnReadOnlyChanged(bool value) => checkList?.SetReadOnly(value);
    private void SetValue() => checkList?.SetValue("孙膑,妲己");
    private string? GetValue() => checkList?.Value;
}