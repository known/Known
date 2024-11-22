namespace Known.Designers;

class FormProperty : BaseProperty<FormFieldInfo>
{
    private List<CodeInfo> customTypes;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        customTypes = Config.FieldTypes.Select(c => new CodeInfo(c.Key, c.Key)).ToList();
    }

    protected override void BuildForm(RenderTreeBuilder builder)
    {
        Model ??= new();
        builder.Div("caption", () => builder.Div("title", $"{Language["Designer.FieldProperty"]} - {Model.Id}"));
        builder.Div("property", () =>
        {
            BuildPropertyItem(builder, nameof(FormFieldInfo.Name), b => b.Span(Model.Name));
            BuildPropertyItem(builder, nameof(FormFieldInfo.Row), b => b.Number(new InputModel<int>
            {
                Disabled = IsReadOnly,
                Value = Model.Row,
                ValueChanged = this.Callback<int>(value => { Model.Row = value; OnChanged?.Invoke(Model); })
            }));
            BuildPropertyItem(builder, nameof(FormFieldInfo.Column), b => b.Number(new InputModel<int>
            {
                Disabled = IsReadOnly,
                Value = Model.Column,
                ValueChanged = this.Callback<int>(value => { Model.Column = value; OnChanged?.Invoke(Model); })
            }));
            BuildPropertyItem(builder, nameof(FormFieldInfo.Span), b => b.Number(new InputModel<int?>
            {
                Disabled = IsReadOnly,
                Value = Model.Span,
                ValueChanged = this.Callback<int?>(value => { Model.Span = value; OnChanged?.Invoke(Model); })
            }));
            BuildPropertyItem(builder, nameof(FormFieldInfo.Type), b => b.Select(new InputModel<string>
            {
                Disabled = IsReadOnly,
                Codes = controlTypes,
                Value = Model.Type.ToString(),
                ValueChanged = this.Callback<string>(value =>
                {
                    if (Model != null)
                    {
                        Model.Type = Utils.ConvertTo<FieldType>(value);
                        if (!Model.Type.HasCategory())
                        {
                            Model.CategoryType = string.Empty;
                            Model.Category = string.Empty;
                        }
                    }
                    OnChanged?.Invoke(Model);
                })
            }));
            if (Model.Type == FieldType.Custom)
            {
                BuildPropertyItem(builder, nameof(FormFieldInfo.CustomField), b => b.Select(new InputModel<string>
                {
                    Disabled = IsReadOnly,
                    Codes = customTypes,
                    Value = Model.CustomField,
                    ValueChanged = this.Callback<string>(value => { Model.CustomField = value; OnChanged?.Invoke(Model); })
                }));
            }
            BuildPropertyItem(builder, nameof(FormFieldInfo.Required), b => b.Switch(new InputModel<bool>
            {
                Disabled = IsReadOnly,
                Value = Model.Required,
                ValueChanged = this.Callback<bool>(value => { Model.Required = value; OnChanged?.Invoke(Model); })
            }));
            BuildPropertyItem(builder, nameof(FormFieldInfo.ReadOnly), b => b.Switch(new InputModel<bool>
            {
                Disabled = IsReadOnly,
                Value = Model.ReadOnly,
                ValueChanged = this.Callback<bool>(value => { Model.ReadOnly = value; OnChanged?.Invoke(Model); })
            }));
            BuildPropertyItem(builder, nameof(FormFieldInfo.Placeholder), b => b.TextBox(new InputModel<string>
            {
                Disabled = IsReadOnly,
                Value = Model.Placeholder,
                ValueChanged = this.Callback<string>(value => { Model.Placeholder = value; OnChanged?.Invoke(Model); })
            }));
            if (Model.Type.HasCategory())
            {
                BuildPropertyItem(builder, nameof(FormFieldInfo.CategoryType), b => b.Select(new InputModel<string>
                {
                    Disabled = IsReadOnly,
                    Codes = Cache.GetCodes("Dictionary,Custom"),
                    Value = Model.CategoryType,
                    ValueChanged = this.Callback<string>(value => { Model.CategoryType = value; OnChanged?.Invoke(Model); })
                }));
                if (Model.CategoryType == "Custom")
                {
                    BuildPropertyItem(builder, nameof(FormFieldInfo.Category), b => b.TextBox(new InputModel<string>
                    {
                        Disabled = IsReadOnly,
                        Value = Model.Category,
                        ValueChanged = this.Callback<string>(value => { Model.Category = value; OnChanged?.Invoke(Model); })
                    }));
                }
                else
                {
                    BuildPropertyItem(builder, nameof(FormFieldInfo.Category), b => b.Select(new InputModel<string>
                    {
                        Disabled = IsReadOnly,
                        Codes = categories,
                        Value = Model.Category,
                        ValueChanged = this.Callback<string>(value => { Model.Category = value; OnChanged?.Invoke(Model); })
                    }));
                }
            }
            if (Model.Type == FieldType.File)
            {
                BuildPropertyItem(builder, nameof(FormFieldInfo.MultiFile), b => b.Switch(new InputModel<bool>
                {
                    Disabled = IsReadOnly,
                    Value = Model.MultiFile,
                    ValueChanged = this.Callback<bool>(value => { Model.MultiFile = value; OnChanged?.Invoke(Model); })
                }));
            }
        });
    }
}