namespace Known.AntBlazor.Components;

public class AntAutoComplete : AutoComplete<string>
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
            emptyText = "请选择或输入";//Item.Language.GetString("PleaseSelect");
        }
        Placeholder = emptyText;
        base.OnInitialized();
    }
}