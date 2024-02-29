namespace Known.AntBlazor.Components;

public class AntNumber<TValue> : InputNumber<TValue>
{
    [CascadingParameter] private IAntForm AntForm { get; set; }
    [CascadingParameter] private DataItem Item { get; set; }

    protected override void OnInitialized()
    {
        if (AntForm != null)
            Disabled = AntForm.IsView;
        if (Item != null)
            Item.Type = typeof(TValue);
        base.OnInitialized();
    }
}