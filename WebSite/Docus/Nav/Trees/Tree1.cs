namespace WebSite.Docus.Nav.Trees;

class Tree1 : BaseComponent
{
    private string? message;
    private readonly List<TreeItem<string>> data = new() {
        new TreeItem<string> {
            Value = "1", Text = "辅助", IsExpanded = true,
            Children = new List<TreeItem<string>> {
                new TreeItem<string> { Value = "11", Text = "孙膑" },
                new TreeItem<string> { Value = "12", Text = "庄周" }
            }
        },
        new TreeItem<string> {
            Value = "2", Text = "射手",
            Children = new List<TreeItem<string>> {
                new TreeItem<string> { Value = "21", Text = "后羿" },
                new TreeItem<string> { Value = "22", Text = "伽罗" }
            }
        }
    };

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Component<Tree<string>>()
               .Set(c => c.Data, data)
               .Set(c => c.ReadOnly, ReadOnly)
               .Set(c => c.OnItemClick, Callback<TreeItem<string>>(OnItemClick))
               .Build();
        builder.Div("tips", message);
    }

    private void OnItemClick(TreeItem<string> item)
    {
        message = $"{item.Value} - {item.Text}";
        StateChanged();
    }
}