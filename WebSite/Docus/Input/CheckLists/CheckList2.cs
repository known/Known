namespace WebSite.Docus.Input.CheckLists;

[Title("控制示例")]
class CheckList2 : BaseComponent
{
    private CheckList? checkList;

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Component<FieldControl>()
               .Set(c => c.OnVisibleChanged, OnVisibleChanged)
               .Set(c => c.OnEnabledChanged, OnEnabledChanged)
               .Set(c => c.SetValue, SetValue)
               .Set(c => c.GetValue, GetValue)
               .Build();

        builder.Field<CheckList>("CheckList")
               .Set(f => f.Codes, "孙膑,后羿,妲己")
               .Build(value => checkList = value);
    }

    private void OnVisibleChanged(string value)
    {
        var visible = Utils.ConvertTo<bool>(value);
        checkList?.SetVisible(visible);
    }

    private void OnEnabledChanged(string value)
    {
        var enabled = Utils.ConvertTo<bool>(value);
        checkList?.SetEnabled(enabled);
    }

    private void SetValue() => checkList?.SetValue("孙膑,妲己");

    private string? GetValue() => checkList?.Value;
}