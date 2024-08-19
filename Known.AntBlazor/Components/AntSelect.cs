namespace Known.AntBlazor.Components;

public class AntSelect : Select<string, string>
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
            Placeholder = Item.Language.GetString("PleaseSelect");
        }
        EnableSearch = true;
        base.OnInitialized();
    }
}

public class AntSelectCode : Select<string, CodeInfo>
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
            Placeholder = emptyText;
        }
        if (!string.IsNullOrWhiteSpace(Category))
            DataSource = Cache.GetCodes(Category).ToCodes(emptyText);
        ValueName = nameof(CodeInfo.Code);
        LabelName = nameof(CodeInfo.Name);
        EnableSearch = true;
        AllowClear = true;
        base.OnInitialized();
    }
}