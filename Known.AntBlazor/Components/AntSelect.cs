namespace Known.AntBlazor.Components;

public class AntSelect : Select<string, string>
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
            emptyText = Item.Language.GetString("PleaseSelect");
        }
        Placeholder = emptyText;
        EnableSearch = true;
        base.OnInitialized();
    }
}

public class AntCodeSelect : Select<string, CodeInfo>
{
    [CascadingParameter] private IAntForm AntForm { get; set; }
    [CascadingParameter] private DataItem Item { get; set; }

    [Parameter] public string Category { get; set; }

    protected override void OnInitialized()
    {
        if (AntForm != null)
            Disabled = AntForm.IsView;
        var emptyText = "";
        if (Item != null)
        {
            Item.Type = typeof(string);
            emptyText = Item.Language.GetString("PleaseSelect");
        }
        if (!string.IsNullOrWhiteSpace(Category))
            DataSource = Cache.GetCodes(Category).ToCodes(emptyText);
        Placeholder = emptyText;
        EnableSearch = true;
        ValueName = nameof(CodeInfo.Code);
        LabelName = nameof(CodeInfo.Name);
        base.OnInitialized();
    }
}