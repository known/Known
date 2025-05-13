namespace Known.Pages;

/// <summary>
/// 多语言管理开发插件页面组件类。
/// </summary>
[Route("/dev/languages")]
[DevPlugin("语言管理", "global", Sort = 2)]
public class LanguagePage : BaseTablePage<LanguageInfo>
{
    private readonly List<LanguageSettingInfo> Infos = Language.Settings;

    /// <inheritdoc />
    protected override async Task OnInitPageAsync()
    {
        if (!CurrentUser.IsSystemAdmin())
        {
            Navigation.GoErrorPage("403");
            return;
        }

        await base.OnInitPageAsync();

        Table = new TableModel<LanguageInfo>(this, TableColumnMode.Attribute);
        Table.Name = PageName;
        Table.FormType = typeof(LanguageForm);
        Table.EnableEdit = false;
        Table.EnableFilter = false;
        Table.AdvSearch = false;
        Table.ShowPager = true;
        Table.SelectType = TableSelectType.Checkbox;
        Table.OnQuery = Platform.QueryLanguagesAsync;

        foreach (var info in Infos)
        {
            if (!info.Enabled)
                continue;

            var property = TypeHelper.Property<LanguageInfo>(info.Id);
            if (property != null)
                Table.AddColumn(property).Name(info.Name);
        }

        Table.Toolbar.ShowCount = 6;
        Table.Toolbar.AddAction(nameof(Setting), Language.TipLanguageSetting);
        Table.Toolbar.AddAction(nameof(Fetch), Language.TipLanguageFetch);
        Table.Toolbar.AddAction(nameof(New));
        Table.Toolbar.AddAction(nameof(DeleteM));
        Table.Toolbar.AddAction(nameof(Import));
        Table.Toolbar.AddAction(nameof(Export));

        Table.AddAction(nameof(Edit));
        Table.AddAction(nameof(Delete));
    }

    /// <summary>
    /// 弹出新增表单对话框。
    /// </summary>
    public void New() => Table.NewForm(Platform.SaveLanguageAsync, new LanguageInfo());

    /// <summary>
    /// 弹出编辑表单对话框。
    /// </summary>
    /// <param name="row">表格行绑定的对象。</param>
    public void Edit(LanguageInfo row) => Table.EditForm(Platform.SaveLanguageAsync, row);

    /// <summary>
    /// 删除一条数据。
    /// </summary>
    /// <param name="row">表格行绑定的对象。</param>
    public void Delete(LanguageInfo row) => Table.Delete(Platform.DeleteLanguagesAsync, row);

    /// <summary>
    /// 批量删除数据。
    /// </summary>
    public void DeleteM() => Table.DeleteM(Platform.DeleteLanguagesAsync);

    /// <summary>
    /// 导入数据。
    /// </summary>
    /// <returns></returns>
    public void Import()
    {
        var form = new FormModel<FileFormInfo>(this)
        {
            Title = Language.GetImportTitle(PageName),
            Data = new FileFormInfo(),
            OnSaveFile = Platform.ImportLanguagesAsync,
            OnSaved = async d => await RefreshAsync()
        };
        form.AddRow().AddColumn(Language.ImportFile, c => c.BizType, c => c.Type = FieldType.File);
        UI.ShowForm(form);
    }

    /// <summary>
    /// 导出数据。
    /// </summary>
    /// <returns></returns>
    public Task Export() => Table.ExportDataAsync();

    /// <summary>
    /// 提取语言。
    /// </summary>
    public void Fetch()
    {
        UI.Confirm(Language.TipLanguageFetchConfirm, async () =>
        {
            var result = await Platform.FetchLanguagesAsync();
            UI.Result(result, () =>
            {
                Language.Datas = result.DataAs<List<LanguageInfo>>();
                return RefreshAsync();
            });
        });
    }

    /// <summary>
    /// 设置语言。
    /// </summary>
    public void Setting()
    {
        LanguageSetting table = null;
        var model = new DialogModel
        {
            Title = Language.SysLanguage,
            Width = 600,
            Content = b => b.Component<LanguageSetting>().Set(c => c.DataSource, Infos).Build(value => table = value)
        };
        model.AddAction(Language.Reset, this.Callback<MouseEventArgs>(e=> table?.Reset()));
        model.OnOk = async () =>
        {
            await Platform.SaveLanguageSettingsAsync(Infos);
            await model.CloseAsync();
            App.ReloadPage();
        };
        UI.ShowDialog(model);
    }
}

class LanguageForm : BaseForm<LanguageInfo>
{
    protected override async Task OnInitFormAsync()
    {
        await base.OnInitFormAsync();

        foreach (var info in Language.Settings)
        {
            if (!info.Enabled)
                continue;

            var property = TypeHelper.Property<LanguageInfo>(info.Id);
            Model.AddRow().AddColumn(property, c => c.Label = info.Name);
        }
    }
}