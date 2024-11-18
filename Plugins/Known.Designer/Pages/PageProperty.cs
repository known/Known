namespace Known.Designer.Pages;

class PageProperty : BaseProperty<PageColumnInfo>
{
    protected override void BuildForm(RenderTreeBuilder builder)
    {
        Model ??= new();
        builder.Div("caption", () => builder.Div("title", $"{Language["Designer.FieldProperty"]} - {Model.Id}"));
        builder.Div("property", () =>
        {
            BuildPropertyItem(builder, "Name", b => b.Span(Model.Name));
            BuildPropertyItem(builder, "IsViewLink", b => b.Switch(new InputModel<bool>
            {
                Disabled = IsReadOnly,
                Value = Model.IsViewLink,
                ValueChanged = this.Callback<bool>(value => { Model.IsViewLink = value; OnChanged?.Invoke(Model); })
            }));
            BuildPropertyItem(builder, "IsQuery", b => b.Switch(new InputModel<bool>
            {
                Disabled = IsReadOnly,
                Value = Model.IsQuery,
                ValueChanged = this.Callback<bool>(value => { Model.IsQuery = value; OnChanged?.Invoke(Model); })
            }));
            if (Model.IsQuery)
            {
                BuildPropertyItem(builder, "IsQueryAll", b => b.Switch(new InputModel<bool>
                {
                    Disabled = IsReadOnly,
                    Value = Model.IsQueryAll,
                    ValueChanged = this.Callback<bool>(value => { Model.IsQueryAll = value; OnChanged?.Invoke(Model); })
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
            }
            BuildPropertyItem(builder, "IsSum", b => b.Switch(new InputModel<bool>
            {
                Disabled = IsReadOnly,
                Value = Model.IsSum,
                ValueChanged = this.Callback<bool>(value => { Model.IsSum = value; OnChanged?.Invoke(Model); })
            }));
            BuildPropertyItem(builder, "IsSort", b => b.Switch(new InputModel<bool>
            {
                Disabled = IsReadOnly,
                Value = Model.IsSort,
                ValueChanged = this.Callback<bool>(value => { Model.IsSort = value; OnChanged?.Invoke(Model); })
            }));
            if (Model.IsSort)
            {
                BuildPropertyItem(builder, "DefaultSort", b => b.Select(new InputModel<string>
                {
                    Disabled = IsReadOnly,
                    Codes = Cache.GetCodes(",Ascend,Descend"),
                    Value = Model.DefaultSort,
                    ValueChanged = this.Callback<string>(value => { Model.DefaultSort = value; OnChanged?.Invoke(Model); })
                }));
            }
            BuildPropertyItem(builder, "Fixed", b => b.Select(new InputModel<string>
            {
                Disabled = IsReadOnly,
                Codes = Cache.GetCodes(",left,right"),
                Value = Model.Fixed,
                ValueChanged = this.Callback<string>(value => { Model.Fixed = value; OnChanged?.Invoke(Model); })
            }));
            BuildPropertyItem(builder, "Width", b => b.Number(new InputModel<int?>
            {
                Disabled = IsReadOnly,
                Value = Model.Width,
                ValueChanged = this.Callback<int?>(value => { Model.Width = value; OnChanged?.Invoke(Model); })
            }));
            BuildPropertyItem(builder, "Align", b => b.Select(new InputModel<string>
            {
                Disabled = IsReadOnly,
                Codes = Cache.GetCodes(",left,center,right"),
                Value = Model.Align,
                ValueChanged = this.Callback<string>(value => { Model.Align = value; OnChanged?.Invoke(Model); })
            }));
        });
    }
}