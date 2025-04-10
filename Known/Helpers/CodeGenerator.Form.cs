namespace Known.Helpers;

partial class CodeGenerator
{
    public string GetForm(FormInfo form, EntityInfo entity)
    {
        var modelName = Model.ModelName;
        if (string.IsNullOrWhiteSpace(modelName))
            modelName = entity.Id;

        var pluralName = GetPluralName(entity.Id);
        var className = DataHelper.GetClassName(entity.Id);
        var sb = new StringBuilder();
        sb.AppendLine("@inherits BaseForm<{0}>", modelName);
        sb.AppendLine("");
        sb.AppendLine("<AntForm Form=\"Model\">");
        var rowNos = form.Fields.Select(c => c.Row).Distinct().OrderBy(r => r).ToList();
        if (rowNos.Count == 1)
        {
            foreach (var item in form.Fields)
            {
                sb.AppendLine("    <AntRow>");
                AppendDataItem(sb, item, 24);
                sb.AppendLine("    </AntRow>");
            }
        }
        else
        {
            foreach (var rowNo in rowNos)
            {
                var fields = form.Fields.Where(c => c.Row == rowNo).OrderBy(c => c.Column).ToList();
                var colSpan = 24 / fields.Count;
                sb.AppendLine("    <AntRow>");
                foreach (var item in fields)
                {
                    var span = item.Span ?? colSpan;
                    AppendDataItem(sb, item, span);
                }
                sb.AppendLine("    </AntRow>");
            }
        }
        sb.AppendLine("</AntForm>");
        return sb.ToString();
    }

    private static void AppendDataItem(StringBuilder sb, FormFieldInfo item, int span)
    {
        var control = GetControlName(item.Type);
        if (item.Required)
            sb.AppendLine("        <DataItem Span=\"{0}\" Label=\"{1}\" Required>", span, item.Name);
        else
            sb.AppendLine("        <DataItem Span=\"{0}\" Label=\"{1}\">", span, item.Name);
        if (item.Type != FieldType.File)
            sb.AppendLine("            <{0} @bind-Value=\"@context.{1}\" />", control, item.Id);
        else
            sb.AppendLine("            <KUpload Value=\"@context.{0}\" IsButton=\"!Model.Data.IsNew\" />", item.Id);
        sb.AppendLine("        </DataItem>");
    }

    private static string GetControlName(FieldType type)
    {
        return type switch
        {
            FieldType.Text => nameof(AntInput),
            FieldType.TextArea => nameof(AntTextArea),
            FieldType.Date => nameof(AntDatePicker),
            FieldType.Number => "AntNumber",
            FieldType.Switch => nameof(AntSwitch),
            FieldType.CheckBox => nameof(AntCheckBox),
            FieldType.CheckList => nameof(AntCheckboxGroup),
            FieldType.RadioList => nameof(AntRadioGroup),
            FieldType.Select => nameof(AntSelect),
            FieldType.Password => nameof(AntPassword),
            FieldType.File => nameof(KUpload),
            FieldType.DateTime => nameof(AntDateTimePicker),
            FieldType.AutoComplete => nameof(AntAutoComplete),
            FieldType.Custom => nameof(AntInput),
            _ => nameof(AntInput),
        };
    }
}