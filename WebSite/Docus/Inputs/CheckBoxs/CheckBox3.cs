namespace WebSite.Docus.Inputs.CheckBoxs;

class CheckBox3 : BaseComponent
{
    private string? message;

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        //ValueChanged事件
        builder.Field<CheckBox>("CheckBox1").ValueChanged(OnValueChanged)
               .Set(f => f.Text, "ValueChanged事件")
               .Build();
        builder.Field<CheckBox>("CheckBox2").ValueChanged(OnValueChanged)
               .Set(f => f.Text, "ValueChanged事件")
               .Set(f => f.Switch, true)
               .Build();
        builder.Div("tips", message);
    }

    private void OnValueChanged(string value)
    {
        message = value;
        StateChanged();
    }
}