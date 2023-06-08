namespace Known.Razor.Components.Fields;

public class Picker : Field
{
    private bool isInitOK;

    [Parameter] public IPicker Pick { get; set; }
    [Parameter] public bool CanEdit { get; set; }

    protected override Task OnInitializedAsync()
    {
        isInitOK = Pick != null && Pick.OnOK != null;
        return base.OnInitializedAsync();
    }

    protected override void BuildInput(RenderTreeBuilder builder)
    {
        builder.Input(attr =>
        {
            attr.Type("text").Id(Id).Name(Id).Disabled(!CanEdit)
                .Value(Value).Required(Required)
                .Add("autocomplete", "off")
                .OnChange(CreateBinder());
        });

        if (Enabled)
        {
            builder.Icon("fa fa-ellipsis-h", attr =>
            {
                attr.OnClick(Callback(e =>
                {
                    if (!isInitOK && Pick != null)
                    {
                        Pick.OnOK = value =>
                        {
                            SetValue(value);
                            OnValueChange();
                        };
                    }
                    UI.Show(Pick);
                }));
            });
        }
    }
}