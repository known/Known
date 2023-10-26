namespace Known.Razor;

public abstract class ListField : Field
{
    [Parameter] public string Codes { get; set; }
    [Parameter] public CodeInfo[] Items { get; set; }
    [Parameter] public Func<CodeInfo[]> CodeAction { get; set; }

    public void SetCodes(string codes)
    {
        Codes = codes;
        StateChanged();
    }

    protected CodeInfo[] ListItems { get; private set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        ListItems = GetListItems();
    }

    protected override void SetContext(FieldContext context)
    {
        base.SetContext(context);
        context.FieldItems = GetListItems();
    }

    private CodeInfo[] GetListItems()
    {
        if (Items != null && Items.Length > 0)
            return Items;

        if (CodeAction != null)
            return CodeAction();

        return CodeInfo.GetCodes(Codes).ToArray();
    }
}