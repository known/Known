namespace Known.Pages;

/// <summary>
/// 多语言管理开发插件页面组件类。
/// </summary>
[Route("/dev/languages")]
[DevPlugin("语言管理", "global", Sort = 2)]
public class LanguagePage : BaseTablePage<SysLanguage>
{
    private ILanguageService Service;
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
        Service = await CreateServiceAsync<ILanguageService>();

        Table = new TableModel<SysLanguage>(this, TableColumnMode.Attribute);
        Table.Name = PageName;
        Table.FormType = typeof(LanguageForm);
        Table.EnableEdit = false;
        Table.EnableFilter = false;
        Table.AdvSearch = false;
        Table.ShowPager = true;
        Table.SelectType = TableSelectType.Checkbox;
        Table.OnQuery = Service.QueryLanguagesAsync;

        foreach (var info in Infos)
        {
            if (!info.Enabled || Table.AllColumns.Exists(d => d.Id == info.Id))
                continue;

            var field = TypeCache.Field(typeof(SysLanguage), info.Id);
            if (field != null)
            {
                var column = field.GetColumn();
                Table.AddColumn(column).Name(info.Name);
            }
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
    public void New() => Table.NewForm(Service.SaveLanguageAsync, new SysLanguage());

    /// <summary>
    /// 弹出编辑表单对话框。
    /// </summary>
    /// <param name="row">表格行绑定的对象。</param>
    public void Edit(SysLanguage row) => Table.EditForm(Service.SaveLanguageAsync, row);

    /// <summary>
    /// 删除一条数据。
    /// </summary>
    /// <param name="row">表格行绑定的对象。</param>
    public void Delete(SysLanguage row) => Table.Delete(Service.DeleteLanguagesAsync, row);

    /// <summary>
    /// 批量删除数据。
    /// </summary>
    public void DeleteM() => Table.DeleteM(Service.DeleteLanguagesAsync);

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
            OnSaveFile = Service.ImportLanguagesAsync,
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
            var result = await Service.FetchLanguagesAsync();
            UI.Result(result, () =>
            {
                Language.Datas = result.DataAs<List<SysLanguage>>();
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
        model.AddAction(Language.Reset, this.Callback<MouseEventArgs>(e => table?.Reset()));
        model.OnOk = async () =>
        {
            await Service.SaveLanguageSettingsAsync(table.DataSource);
            await model.CloseAsync();
            App.ReloadPage();
        };
        UI.ShowDialog(model);
    }
}

class LanguageForm : BaseForm<SysLanguage>
{
    protected override async Task OnInitFormAsync()
    {
        await base.OnInitFormAsync();

        foreach (var info in Language.Settings)
        {
            if (!info.Enabled)
                continue;

            var field = TypeCache.Field(typeof(SysLanguage), info.Id);
            var column = field.GetForm();
            Model.AddRow().AddColumn(column, c => c.Label = info.Name);
        }
    }
}