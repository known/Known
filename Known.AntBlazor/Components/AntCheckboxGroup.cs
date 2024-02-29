namespace Known.AntBlazor.Components;

public class AntCheckboxGroup : CheckboxGroup
{
    [CascadingParameter] private IAntForm AntForm { get; set; }
    [CascadingParameter] private DataField Field { get; set; }

    [Parameter] public List<CodeInfo> Codes { get; set; }

    protected override void OnInitialized()
    {
        if (AntForm != null)
            Disabled = AntForm.IsView;
        if (Field != null)
            Field.Type = typeof(string);
        base.OnInitialized();
    }

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
        Options = Codes.ToCheckboxOptions(o =>
        {
            o.Checked = Value != null && Value.Contains(o.Value);
        });
    }
}