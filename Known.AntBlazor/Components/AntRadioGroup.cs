namespace Known.AntBlazor.Components;

public class AntRadioGroup : RadioGroup<string>
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
        //Fixed单选按钮组切换不刷新问题
        OnChange = EventCallback.Factory.Create<string>(this, value => StateHasChanged());
    }

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
        Options = Codes.ToRadioOptions();
    }
}