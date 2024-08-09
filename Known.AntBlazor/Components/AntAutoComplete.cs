namespace Known.AntBlazor.Components;

public class AntAutoComplete : AutoComplete<CodeInfo>
{
    [CascadingParameter] private IAntForm AntForm { get; set; }
    [CascadingParameter] private DataItem Item { get; set; }

    protected override void OnInitialized()
    {
        if (AntForm != null)
            Disabled = AntForm.IsView;
        var emptyText = "";
        if (Item != null)
        {
            Item.Type = typeof(string);
            emptyText = Item.Language.GetString("PleaseSelectInput");
        }
        Placeholder = emptyText;
        OptionFormat = item => item.Value.Name;
        base.OnInitialized();
    }
}