namespace Known;

partial class CodeGenerator
{
    public string GetForm(FormInfo form, EntityInfo entity)
    {
        var pluralName = GetPluralName(entity.Id);
        var className = AdminHelper.GetClassName(entity.Id);
        var sb = new StringBuilder();
        sb.AppendLine("@inherits BaseForm<{0}.Entities.{1}>", Config.App.Id, entity.Id);
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
            sb.AppendLine("            <KUpload Value=\"@context.{0}\" ReadOnly=\"Model.IsView\" IsButton=\"!Model.Data.IsNew\" />", item.Id);
        sb.AppendLine("        </DataItem>");
    }

    private static string GetControlName(FieldType type)
    {
        switch (type)
        {
            case FieldType.Text:
                return "AntInput";
            case FieldType.TextArea:
                return "AntTextArea";
            case FieldType.Date:
                return "AntDatePicker";
            case FieldType.Number:
                return "AntNumber";
            case FieldType.Switch:
                return "AntSwitch";
            case FieldType.CheckBox:
                return "AntCheckBox";
            case FieldType.CheckList:
                return "AntCheckboxGroup";
            case FieldType.RadioList:
                return "AntRadioGroup";
            case FieldType.Select:
                return "AntSelect";
            case FieldType.Password:
                return "AntPassword";
            case FieldType.File:
                return "KUpload";
            case FieldType.DateTime:
                return "AntDateTimePicker";
            case FieldType.AutoComplete:
                return "AntAutoComplete";
            case FieldType.Custom:
                return "AntInput";
            default:
                return "AntInput";
        }
    }
}