namespace Known.AntBlazor.Components;

public class AntDatePicker<TValue> : DatePicker<TValue>
{
    [CascadingParameter] private IAntForm AntForm { get; set; }
    [CascadingParameter] private DataField Field { get; set; }

    protected override void OnInitialized()
    {
        if (AntForm != null)
            Disabled = AntForm.IsView;
        if (Field != null)
            Field.Type = typeof(TValue);
        base.OnInitialized();
    }
}