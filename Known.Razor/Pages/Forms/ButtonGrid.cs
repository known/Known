namespace Known.Razor.Pages.Forms;

class ButtonGrid : EditGrid<ButtonInfo>
{
    internal const string KeyButton = "按钮配置";
    internal const string KeyAction = "操作配置";

    public ButtonGrid()
    {
        Style = "form-grid";
        Data = new List<ButtonInfo>();
        var builder = new ColumnBuilder<ButtonInfo>();
        builder.Field(f => f.Name).Name("按钮").ReadOnly();
        Columns = builder.ToColumns();
    }

    [Parameter] public string Key { get; set; }
    [Parameter] public string Value { get; set; }
    [Parameter] public Action<string> OnValueChanged { get; set; }

    protected override Task OnInitializedAsync()
    {
        var buttons = GetButtons(Value);
        if (buttons != null && buttons.Count > 0)
            Data.AddRange(buttons);
        return base.OnInitializedAsync();
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        base.BuildRenderTree(builder);
        builder.Div("form-button", attr =>
        {
            builder.Button(FormButton.OK, Callback(OnOK));
            builder.Button(FormButton.Cancel, Callback(OnCancel));
        });
    }

    protected override void OnAdd()
    {
        var buttons = GetButtons();
        var items = buttons.Where(b => !Data.Contains(b)).Select(b => new CodeInfo(b.Name, b.Name)).ToArray();
        UI.Prompt("添加", new(550, 350), builder =>
        {
            builder.Field<CheckList>("Data")
                   .Set(f => f.Items, items)
                   .Set(f => f.ColumnCount, 3)
                   .Build();
        }, model =>
        {
            string value = model.Data;
            var buttons = GetButtons(value);
            if (buttons != null && buttons.Count > 0)
            {
                Data.AddRange(buttons);
                SetValue();
                StateChanged();
            }
            UI.CloseDialog();
        });
    }

    private void OnOK()
    {
        SetValue();
        OnValueChanged?.Invoke(Value);
        OnCancel();
    }

    private void OnCancel() => UI.CloseDialog();
    private void SetValue() => Value = string.Join(",", Data?.Select(b => b.Name));
    private List<ButtonInfo> GetButtons() => Key == KeyButton ? ToolButton.Buttons : GridAction.Actions;

    private List<ButtonInfo> GetButtons(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        var infos = new List<ButtonInfo>();
        var buttons = GetButtons();
        var values = value.Split(',').ToList();
        foreach (var item in values)
        {
            var info = buttons.FirstOrDefault(b => b.Name == item);
            infos.Add(info);
        }
        return infos;
    }
}