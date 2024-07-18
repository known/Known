namespace Known.Components;

public class KField<TItem> : ComponentBase where TItem : class, new()
{
    [Inject] public IUIService UI { get; set; }
    [Parameter] public FieldModel<TItem> Model { get; set; }

    //protected override void OnParametersSet()
    //{
    //    base.OnParametersSet();
    //    Model.OnStateChanged = StateChanged;
    //}

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (Model.Column.Template != null)
        {
            builder.Fragment(Model.Column.Template);
        }
        else if (Model.Column.Type == FieldType.File)
        {
            builder.Component<KUploadField<TItem>>().Set(c => c.Model, Model).Build();
        }
        else
        {
            var dataType = Model.GetPropertyType();
            var inputType = UI.GetInputType(dataType, Model.Column.Type);
            if (inputType != null)
            {
                builder.OpenComponent(0, inputType);
                builder.AddMultipleAttributes(1, Model.InputAttributes);
                builder.CloseComponent();
            }
        }
    }
}