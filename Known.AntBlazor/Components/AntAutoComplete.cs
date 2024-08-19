namespace Known.AntBlazor.Components;

public class AntAutoComplete : AutoComplete<CodeInfo>
{
    [CascadingParameter] private IAntForm AntForm { get; set; }
    [CascadingParameter] private DataItem Item { get; set; }

    protected override void OnInitialized()
    {
        if (AntForm != null)
            Disabled = AntForm.IsView;
        if (Item != null)
        {
            Item.Type = typeof(string);
            Placeholder = Item.Language.GetString("PleaseSelectInput");
        }
        OptionFormat = item => item.Value.Name;
        base.OnInitialized();
    }
}