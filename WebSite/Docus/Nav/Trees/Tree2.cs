namespace WebSite.Docus.Nav.Trees;

class Tree2 : BaseComponent
{
    private string? message;
    private HashSet<string> value = new string[] { "11", "22" }.ToHashSet();
    private readonly List<KTreeItem<string>> data = new() {
        new KTreeItem<string> {
            Value = "1", Text = "辅助", IsExpanded = true,
            Children = new List<KTreeItem<string>> {
                new KTreeItem<string> { Value = "11", Text = "孙膑" },
                new KTreeItem<string> { Value = "12", Text = "庄周" }
            }
        },
        new KTreeItem<string> {
            Value = "2", Text = "射手",
            Children = new List<KTreeItem<string>> {
                new KTreeItem<string> { Value = "21", Text = "后羿" },
                new KTreeItem<string> { Value = "22", Text = "伽罗" }
            }
        }
    };

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Component<KTree<string>>()
               .Set(c => c.Data, data)
               .Set(c => c.ReadOnly, ReadOnly)
               .Set(c => c.ShowCheckBox, true)
               .Set(c => c.Values, value)
               .Set(c => c.OnItemClick, Callback<KTreeItem<string>>(OnItemClick))
               .Set(c => c.ValuesChanged, Callback<HashSet<string>>(v => value = v))
               .Build();
        builder.Button("赋值", Callback(OnSetValue), StyleType.Primary);
        builder.Button("取值", Callback(OnGetValue), StyleType.Primary);
        builder.Div("tips", message);
    }

    private void OnItemClick(KTreeItem<string> item)
    {
        message = $"{item.Value} - {item.Text}";
        StateChanged();
    }

    private void OnSetValue()
    {
        value = new string[] { "11", "12", "22" }.ToHashSet();
        StateChanged();
    }

    private void OnGetValue()
    {
        message = string.Join(",", value.ToList());
        StateChanged();
    }
}