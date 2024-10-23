namespace Known.Designers;

class BaseView<TModel> : BaseComponent
{
    protected TabModel Tab { get; } = new();
    [Inject] internal ICodeGenerator Generator { get; set; }
    [CascadingParameter] internal ModuleForm Form { get; set; }

    [Parameter] public TModel Model { get; set; }
    [Parameter] public Action<TModel> OnChanged { get; set; }

    internal string ModulePath => Config.App.ContentRoot.Replace(".Web", "");

    internal virtual void SetModel(TModel model) => Model = model;
    protected override void BuildRender(RenderTreeBuilder builder) => UI.BuildTabs(builder, Tab);

    protected void BuildList<TItem>(RenderTreeBuilder builder, TableModel<TItem> model) where TItem : class, new()
    {
        builder.Div("list-view", () => UI.BuildTable(builder, model));
    }

    protected void BuildAction(RenderTreeBuilder builder, string button, Action action)
    {
        if (ReadOnly)
            return;

        builder.Div("kui-code-action", () =>
        {
            builder.Button(button, this.Callback<MouseEventArgs>(e => action()));
        });
    }

    protected void BuildCode(RenderTreeBuilder builder, string type, string path, string code)
    {
        if (!string.IsNullOrWhiteSpace(path))
            builder.Div($"kui-code-path {type}", () => builder.Tag(path));

        var html = $"<pre class=\"highlight kui-code {type} language-csharp\"><code>{code}</code></pre>";
        builder.Markup(html);
    }

    protected void BuildPropertyItem(RenderTreeBuilder builder, string label, Action<RenderTreeBuilder> template)
    {
        builder.Div("item", () =>
        {
            if (!string.IsNullOrWhiteSpace(label))
                builder.Label(Language[label]);
            builder.Div(() => template?.Invoke(builder));
        });
    }

    internal void SaveSourceCode(string path, string code)
    {
        if (string.IsNullOrWhiteSpace(path))
            return;
#if DEBUG
        if (File.Exists(path))
        {
            UI.Confirm($"文件[{path}]已存在，确定要覆盖吗？", () =>
            {
                Utils.SaveFile(path, code);
                return UI.Toast("保存成功！");
            });
        }
        else
        {
            Utils.SaveFile(path, code);
            UI.Toast("保存成功！");
        }
#endif
    }
}