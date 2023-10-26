using Known.Extensions;

namespace Known.Razor;

public class KCheckBox : Field
{
    [Parameter] public string Text { get; set; }
    [Parameter] public bool Switch { get; set; }

    protected override void BuildText(RenderTreeBuilder builder)
    {
        BuildCheckBox(builder, "True", false, IsChecked);
    }

    protected override void BuildInput(RenderTreeBuilder builder)
    {
        BuildCheckBox(builder, "True", Enabled, IsChecked);
    }

    private bool IsChecked => Value == "True";

    private void BuildCheckBox(RenderTreeBuilder builder, string value, bool enabled, bool isChecked)
    {
        builder.Label("form-radio", attr =>
        {
            builder.Input(attr =>
            {
                var css = CssBuilder.Default("").AddClass("switch", Switch).Build();
                attr.Type("checkbox").Name(Id).Disabled(!enabled)
                    .Value(value).Checked(isChecked).Class(css)
                    .OnChange(EventCallback.Factory.CreateBinder<bool>(this, isCheck =>
                    {
                        Value = isCheck ? "True" : "False";
                        OnValueChange();
                    }, isChecked));
            });
            if (Switch)
                builder.Span(attr => builder.Text(Text));
            else
                builder.Span(Text);
        });
    }
}