namespace Known.Designers;

class FormView : BaseView<FormInfo>
{
    private readonly TabModel tab = new();
    private FormModel<Dictionary<string, object>> form;
    private TableModel<FormFieldInfo> list;
    private string codeForm;

    [Parameter] public FlowInfo Flow { get; set; }

    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();

        form = new FormModel<Dictionary<string, object>>(this) { Data = [] };
        SetForm();

        Tab.AddTab("Designer.View", BuildView);
        Tab.AddTab("Designer.Fields", BuildList);
        if (IsCustomPage)
            Tab.AddTab("Designer.FormCode", BuildForm);

        list = new(this, TableColumnMode.Property)
        {
            ShowSetting = false,
            FixedHeight = "355px",
            OnQuery = c =>
            {
                var result = new PagingResult<FormFieldInfo>(Model?.Fields);
                return Task.FromResult(result);
            }
        };

        tab.AddTab("Designer.Property", BuildProperty);
    }

    internal override async Task SetModelAsync(FormInfo model)
    {
        await base.SetModelAsync(model);
        SetForm();
        await StateChangedAsync();
        await list.RefreshAsync();
    }

    private void BuildView(RenderTreeBuilder builder)
    {
        builder.Div("view", () =>
        {
            var steps = Flow?.GetFlowStepItems();
            if (steps != null && steps.Count > 0)
            {
                var step = new StepModel();
                step.Items.AddRange(steps);
                builder.Steps(step);
            }
            builder.Form(form);
        });
        builder.Div("setting", () => builder.Tabs(tab));
    }

    private void BuildList(RenderTreeBuilder builder) => BuildList(builder, list);

    private void BuildForm(RenderTreeBuilder builder)
    {
        var modulePath = AdminOption.Instance.Code?.PagePath ?? ModulePath;
        var className = DataHelper.GetClassName(Module?.Entity?.Id);
        var path = Path.Combine(modulePath, "Pages", "Forms", $"{className}Form.razor");
        if (Config.IsDebug)
            BuildAction(builder, Language.Save, () => SaveSourceCode(path, codeForm));
        BuildCode(builder, "page", path, codeForm, "html");
    }

    private void BuildProperty(RenderTreeBuilder builder)
    {
        builder.Div("setting-row", () =>
        {
            BuildPropertyItem(builder, "Designer.Maximizable", b => b.Switch(new InputModel<bool>
            {
                Disabled = ReadOnly,
                Value = Model.Maximizable,
                ValueChanged = this.Callback<bool>(value =>
                {
                    Model.Maximizable = value;
                    return OnPropertyChangedAsync();
                })
            }));
            BuildPropertyItem(builder, "Designer.DefaultMaximized", b => b.Switch(new InputModel<bool>
            {
                Disabled = ReadOnly,
                Value = Model.DefaultMaximized,
                ValueChanged = this.Callback<bool>(value =>
                {
                    Model.DefaultMaximized = value;
                    return OnPropertyChangedAsync();
                })
            }));
            BuildPropertyItem(builder, "Designer.IsContinue", b => b.Switch(new InputModel<bool>
            {
                Disabled = ReadOnly,
                Value = Model.IsContinue,
                ValueChanged = this.Callback<bool>(value =>
                {
                    Model.IsContinue = value;
                    return OnPropertyChangedAsync();
                })
            }));
            BuildPropertyItem(builder, "Width", b => b.Number(new InputModel<double?>
            {
                Disabled = ReadOnly,
                Value = Model.Width,
                ValueChanged = this.Callback<double?>(value =>
                {
                    Model.Width = value;
                    return OnPropertyChangedAsync();
                })
            }));
        });
    }

    private async Task OnPropertyChangedAsync()
    {
        await SetModelAsync(Model);
        OnChanged?.Invoke(Model);
    }

    private void SetForm()
    {
        form.SetFormInfo(Model);
        form.InitColumns();

        if (IsCustomPage)
            codeForm = Generator?.GetForm(Model, Module?.Entity);
    }
}