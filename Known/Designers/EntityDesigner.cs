namespace Known.Designers;

class EntityDesigner : BaseDesigner<string>
{
    private string dataTypes;
    private List<CodeInfo> addTypes;
    private List<CodeInfo> entityModels;
    private string addType;
    private EntityInfo entity = new();
    private EntityView view;

    private bool IsNew => addType == addTypes[0].Code;

    protected override async Task OnInitAsync()
    {
        addTypes =
        [
            new CodeInfo("New", Language["Designer.New"]),
            new CodeInfo("Select", Language["Designer.SelectEntity"])
        ];
        await base.OnInitAsync();
        entityModels = DataHelper.Models.Select(m => new CodeInfo(m.Id, $"{m.Name}({m.Id})", m)).ToList();
        dataTypes = string.Join(",", Cache.GetCodes(nameof(FieldType)).Select(c => c.Name));
        addType = string.IsNullOrWhiteSpace(Model) || Model.Contains('|')
                ? addTypes[0].Code : addTypes[1].Code;
        entity = DataHelper.GetEntity(Model);
        Form.Entity = entity;
    }

    protected override void BuildRender(RenderTreeBuilder builder)
    {
        builder.Div("kui-designer entity", () =>
        {
            builder.Div("panel-model", () =>
            {
                builder.Div("caption", () =>
                {
                    builder.Div("title", Language["Designer.Models"]);
                    BuildModelType(builder);
                });
                BuildNewModel(builder);
            });
            builder.Div("panel-view", () =>
            {
                builder.Component<EntityView>()
                       .Set(c => c.ReadOnly, ReadOnly)
                       .Set(c => c.Model, entity)
                       .Build(value => view = value);
            });
        });
    }

    private void BuildModelType(RenderTreeBuilder builder)
    {
        builder.Div(() =>
        {
            UI.BuildRadioList(builder, new InputModel<string>
            {
                Disabled = ReadOnly,
                Codes = addTypes,
                Value = addType,
                ValueChanged = this.Callback<string>(OnTypeChanged)
            });
        });

        if (!IsNew)
        {
            builder.Div("select", () =>
            {
                UI.BuildSelect(builder, new InputModel<string>
                {
                    Disabled = ReadOnly,
                    Codes = entityModels,
                    Value = Model,
                    ValueChanged = this.Callback<string>(OnModelChanged)
                });
            });
        }
    }

    private void BuildNewModel(RenderTreeBuilder builder)
    {
        UI.BuildTextArea(builder, new InputModel<string>
        {
            Disabled = ReadOnly || !IsNew,
            Rows = 11,
            Value = Model,
            ValueChanged = this.Callback<string>(OnModelChanged)
        });
    }

    private void ShowTips(RenderTreeBuilder builder)
    {
        builder.Markup($@"<pre><b>{Language["Designer.Explanation"]}</b>
{Language["Designer.Entity"]}{Language["Name"]}|{Language["Code"]}|{Language["Designer.FlowClass"]}
{Language["Designer.Field"]}{Language["Name"]}|{Language["Code"]}|{Language["Type"]}|{Language["Length"]}|{Language["Required"]}
{Language["Designer.Type"]}{dataTypes}
<b>{Language["Designer.Sample"]}</b>
{Language["Designer.Test"]}|KmTest|Y
{Language["Designer.Text"]}|Field1|Text|50|Y
{Language["Designer.Number"]}|Field2|Number|18,5
{Language["Designer.Date"]}|Field3|Date</pre>");
    }

    private void OnTypeChanged(string type) => addType = type;

    private void OnModelChanged(string model)
    {
        Model = model;
        entity = DataHelper.GetEntity(model);
        Form.Entity = entity;
        view?.SetModel(entity);
        OnChanged?.Invoke(model);
    }
}