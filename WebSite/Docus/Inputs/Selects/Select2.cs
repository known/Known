namespace WebSite.Docus.Inputs.Selects;

class Select2 : BaseComponent
{
    private string? message;

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        //ValueChanged事件
        builder.Field<Select>("英雄：", "Select").ValueChanged(OnValueChanged)
               .Set(f => f.Codes, "孙膑,后羿,妲己")
               .Build();
        builder.Div("tips", message);
    }

    private void OnValueChanged(string value)
    {
        message = value;
        StateChanged();
    }
}