namespace Known.Designers;

class BaseView<TModel> : BaseComponent
{
    protected TabModel Tab { get; } = new();
    [Inject] internal ICodeGenerator Generator { get; set; }

    [Parameter] public SysModule Module { get; set; }
    [Parameter] public TModel Model { get; set; }
    [Parameter] public Action<TModel> OnChanged { get; set; }

    internal bool IsCustomPage => Module.IsCustomPage;
    internal string ModulePath => Config.IsDebug ? Config.App.ContentRoot : "";

    internal virtual Task SetModelAsync(TModel model)
    {
        Model = model;
        return Task.CompletedTask;
    }

    protected override void BuildRender(RenderTreeBuilder builder) => builder.Tabs(Tab);

    protected void BuildList<TItem>(RenderTreeBuilder builder, TableModel<TItem> model) where TItem : class, new()
    {
        builder.Div("list-view", () => builder.Table(model));
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

    protected void BuildCode(RenderTreeBuilder builder, string type, string path, string code, string lang = "csharp")
    {
        if (!string.IsNullOrWhiteSpace(path))
            builder.Div($"kui-code-path {type}", () => builder.Tag(path));

        builder.Component<KCodeView>()
               .Set(c => c.Class, type)
               .Set(c => c.Code, code)
               .Set(c => c.Lang, lang)
               .Build();
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

        if (!Config.IsDebug)
            return;

        if (File.Exists(path))
        {
            UI.Confirm($"文件[{path}]已存在，确定要覆盖吗？", async () =>
            {
                await Utils.SaveFileAsync(path, code);
                await UI.SuccessAsync("保存成功！");
            });
        }
        else
        {
            Utils.SaveFile(path, code);
            UI.Success("保存成功！");
        }
    }
}