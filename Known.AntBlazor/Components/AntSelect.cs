namespace Known.AntBlazor.Components;

public class AntSelect : Select<string, CodeInfo>
{
    [CascadingParameter] private IAntForm AntForm { get; set; }
    [CascadingParameter] private DataItem Item { get; set; }

    [Parameter] public string Category { get; set; }

    protected override void OnInitialized()
	{
        if (AntForm != null)
            Disabled = AntForm.IsView;
        if (Item != null)
            Item.Type = typeof(string);
        ValueName = nameof(CodeInfo.Code);
		LabelName = nameof(CodeInfo.Name);
        if (!string.IsNullOrWhiteSpace(Category))
            DataSource = Cache.GetCodes(Category).ToCodes("请选择");
        Placeholder = "请选择";
        base.OnInitialized();
	}
}