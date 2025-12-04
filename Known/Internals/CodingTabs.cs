namespace Known.Internals;

/// <summary>
/// 生成代码标签页组件类。
/// </summary>
public class CodingTabs : BaseComponent
{
    private TabModel Tab;
    private ICodeService Service;
    private bool isAutoMode = Config.RenderMode == RenderType.Auto;
    private string firstTab = "";
    private string currentTab = "";

    [Inject] private ICodeGenerator Generator { get; set; }

    /// <summary>
    /// 取得或设置标题。
    /// </summary>
    [Parameter] public string Title { get; set; }

    /// <summary>
    /// 取得或设置代码生成模型信息。
    /// </summary>
    [Parameter] public CodeModelInfo Model { get; set; }

    /// <summary>
    /// 取得或设置模型配置标签字典。
    /// </summary>
    [Parameter] public Dictionary<string, RenderFragment> ModelTabs { get; set; }

    /// <summary>
    /// 取得或设置标签改变委托。
    /// </summary>
    [Parameter] public Action<string> OnTabChange { get; set; }

    /// <summary>
    /// 设置代码配置模型信息。
    /// </summary>
    /// <param name="model">代码配置模型信息。</param>
    public void SetModel(CodeModelInfo model)
    {
        Model = model;
    }

    /// <inheritdoc />
    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        Service = await CreateServiceAsync<ICodeService>();

        firstTab = ModelTabs.Keys.First();
        currentTab = firstTab;
        Tab = new TabModel(this) { Class = Class };
        foreach (var tab in ModelTabs)
        {
            Tab.AddTab(tab.Key, tab.Value);
        }
        Tab.AddTab(CodeTab.Script, BuildCode);
        Tab.AddTab(CodeTab.Info, BuildCode);
        Tab.AddTab(CodeTab.Entity, BuildCode);
        Tab.AddTab(CodeTab.Page, BuildCode);
        Tab.AddTab(CodeTab.Form, BuildCode);
        Tab.AddTab(CodeTab.ServiceI, BuildCode);
        Tab.AddTab(CodeTab.Service, BuildCode);

        Tab.OnChange = tab =>
        {
            currentTab = tab;
            OnTabChange?.Invoke(tab);
            StateChanged();
        };
        if (!string.IsNullOrWhiteSpace(Title))
            Tab.Left = b => b.FormTitle(Title);
        Tab.Right = BuildTabRight;
    }

    /// <inheritdoc />
    protected override void BuildRender(RenderTreeBuilder builder)
    {
        foreach (var item in Tab.Items)
        {
            if (item.Id == CodeTab.Info || item.Id == CodeTab.ServiceI)
                item.IsVisible = isAutoMode;
        }

        if (!string.IsNullOrWhiteSpace(Title))
            builder.Div("kui-card", () => builder.Tabs(Tab));
        else
            builder.Tabs(Tab);
    }

    private void BuildTabRight(RenderTreeBuilder builder)
    {
        if (currentTab == firstTab)
        {
            builder.Component<AntSwitch>()
                   .Set(c => c.Value, isAutoMode)
                   .Set(c => c.ValueChanged, this.Callback<bool>(value => isAutoMode = value))
                   .Set(c => c.ShowTexts, "Auto模式,Server模式")
                   .Build();
        }

        if (ModelTabs.ContainsKey(currentTab))
            return;

        if (currentTab == CodeTab.Script)
        {
            builder.Button(Language.Execute, this.Callback<MouseEventArgs>(OnExecute));
        }
        else
        {
            var path = GetCodePath(currentTab);
            var file = GetCodeFile(currentTab);
            builder.Tooltip(path, b => b.Tag(file));
            builder.Button(Language.Save, this.Callback<MouseEventArgs>(OnSaveCode));
        }
    }

    private void BuildCode(RenderTreeBuilder builder)
    {
        var lang = GetLanguage(currentTab);
        var code = GenerateCode(currentTab);
        builder.Component<KCodeView>().Set(c => c.Lang, lang).Set(c => c.Code, code).Build();
    }

    private async Task OnExecute(MouseEventArgs args)
    {
        var code = GenerateCode(currentTab);
        var info = new AutoInfo<string> { PageId = Model.EntityName, Data = code };
        var result = await Service.CreateTableAsync(info);
        UI.Result(result);
    }

    private async Task OnSaveCode(MouseEventArgs args)
    {
        var path = GetCodePath(currentTab);
        var code = GenerateCode(currentTab);
        var info = new AutoInfo<string> { PageId = path, PluginId = currentTab, Data = code };
        var result = await Service.SaveCodeAsync(info);
        UI.Result(result);
    }

    private static string GetLanguage(string name)
    {
        return name switch
        {
            CodeTab.Form => "html",
            _ => "csharp"
        };
    }

    private string GetCodePath(string name)
    {
        if (Model == null)
            return string.Empty;

        return name switch
        {
            CodeTab.Info => Model.ModelPath,
            CodeTab.Entity => Model.EntityPath,
            CodeTab.Page => Model.PagePath,
            CodeTab.Form => Model.FormPath,
            CodeTab.ServiceI => Model.ServiceIPath,
            CodeTab.Service => Model.ServicePath,
            _ => ""
        };
    }

    private string GetCodeFile(string name)
    {
        if (Model == null)
            return string.Empty;

        return name switch
        {
            CodeTab.Info => $"{Model.ModelName}.cs",
            CodeTab.Entity => $"{Model.EntityName}.cs",
            CodeTab.Page => $"{Model.PageName}.cs",
            CodeTab.Form => $"{Model.FormName}.razor",
            CodeTab.ServiceI => $"{Model.ServiceName}.cs",
            CodeTab.Service => $"{Model.ServiceName}.cs",
            _ => ""
        };
    }

    private string GenerateCode(string name)
    {
        if (Model == null)
            return string.Empty;

        Model.IsAutoMode = isAutoMode;
        Generator.Model = Model;
        return name switch
        {
            CodeTab.Script => Generator.GetScript(Config.DatabaseType, Model.Entity),
            CodeTab.Info => Generator.GetModel(Model.Entity),
            CodeTab.Entity => Generator.GetEntity(Model.Entity),
            CodeTab.Page => Generator.GetPage(Model.Page, Model.Entity),
            CodeTab.Form => Generator.GetForm(Model.Form, Model.Entity),
            CodeTab.ServiceI => Generator.GetIService(Model.Page, Model.Entity, true),
            CodeTab.Service => Generator.GetService(Model.Page, Model.Entity),
            _ => ""
        };
    }
}