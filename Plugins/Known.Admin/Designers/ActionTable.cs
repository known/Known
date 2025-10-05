namespace Known.Designers;

class ActionTable : BaseTable<ActionInfo>
{
    private List<ActionInfo> actions;

    [Parameter] public string Type { get; set; }
    [Parameter] public List<string> Value { get; set; }

    public List<string> Values => Table.DataSource?.Select(d => d.Id).ToList();

    protected override async Task OnInitAsync()
    {
        actions = [.. Config.Actions];
        actions.ForEach(a => a.Name = Language.GetString(a));

        await base.OnInitAsync();
        Table.AutoHeight = false;
        Table.Name = Name;
        Table.ActionWidth = "200";
        Table.ActionCount = 3;
        Table.Toolbar.AddAction(nameof(Add));
        Table.DataSource = Value?.Select(v => actions.FirstOrDefault(a => a.Id == v)).ToList() ?? [];
        Table.AddColumn(c => c.Name).Template((b, r) => b.IconName(r.Icon, r.Name));
        Table.AddAction(nameof(Delete));
        Table.AddAction(nameof(MoveUp));
        Table.AddAction(nameof(MoveDown));
    }

    public void Add()
    {
        string[] values = [];
        var model = new DialogModel
        {
            Title = Name,
            Content = b => b.CheckList(new InputModel<string[]>
            {
                Codes = actions.Where(a => a.HasType(Type) && (Value == null || !Value.Contains(a.Id)))
                               .Select(a => new CodeInfo(a.Id, a.Name))
                               .ToList(),
                ValueChanged = this.Callback<string[]>(value => values = value)
            })
        };
        model.OnOk = async () =>
        {
            //TODO: 添加按钮后，导致上级Dialog无法关闭
            var items = actions.Where(a => values.Contains(a.Id)).ToList();
            var datas = Table.DataSource;
            datas?.AddRange(items);
            Table.DataSource = datas;
            await Table.RefreshAsync();
            await model.CloseAsync();
        };
        UI.ShowDialog(model);
    }

    public void Delete(ActionInfo row) => DeleteRow(row);
}