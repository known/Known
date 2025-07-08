namespace Known.Helpers;

partial class CodeGenerator
{
    public string GetEntity(EntityInfo entity)
    {
        if (entity == null)
            return string.Empty;

        var baseType = string.Empty;
        if (entity.IsEntity)
            baseType = " : EntityBase";
        if (entity.IsFlow)
            baseType = " : FlowEntity";

        var entityName = Model.EntityName;
        if (string.IsNullOrWhiteSpace(entityName))
            entityName = entity.Id;

        var sb = new StringBuilder();
        sb.AppendLine("namespace {0}.Entities;", Model.Namespace);
        sb.AppendLine(" ");
        sb.AppendLine("/// <summary>");
        sb.AppendLine("/// {0}实体类。", entity.Name);
        sb.AppendLine("/// </summary>");
        sb.AppendLine("[DisplayName(\"{0}\")]", entity.Name);
        sb.AppendLine("public class {0}{1}", entityName, baseType);
        sb.AppendLine("{");
        AppendFields(sb, entity, !Model.IsAutoMode);
        sb.AppendLine("}");
        return sb.ToString();
    }

    private static void AppendFields(StringBuilder sb, EntityInfo entity, bool addPage)
    {
        var index = 0;
        foreach (var item in entity.Fields)
        {
            if (index++ > 0)
                sb.AppendLine(" ");

            var type = GetCSharpType(item);
            if (type != "bool" && type != "string")
                type += "?";

            sb.AppendLine("    /// <summary>");
            sb.AppendLine("    /// 取得或设置{0}。", item.Name);
            sb.AppendLine("    /// </summary>");
            if (item.IsKey)
                sb.AppendLine("    [Required, Key]");
            else if (item.Required)
                sb.AppendLine("    [Required]");
            if (!string.IsNullOrWhiteSpace(item.Length) && type == "string")
                sb.AppendLine("    [MaxLength({0})]", item.Length);
            if (item.IsGrid && addPage)
                sb.AppendLine("    [Column(Width = 100)]");
            if (item.IsForm && addPage)
            {
                if (item.Type == FieldType.File)
                    sb.AppendLine("    [Form(Type = nameof(FieldType.File))]");
                else if (item.Type == FieldType.Switch)
                    sb.AppendLine("    [Form(Type = nameof(FieldType.Switch))]");
                else if (item.Type == FieldType.Date)
                    sb.AppendLine("    [Form(Type = nameof(FieldType.Date))]");
                else if (item.Type == FieldType.DateTime)
                    sb.AppendLine("    [Form(Type = nameof(FieldType.DateTime))]");
                else if (item.Type == FieldType.TextArea)
                    sb.AppendLine("    [Form(Type = nameof(FieldType.TextArea))]");
                else
                    sb.AppendLine("    [Form]");
            }
            sb.AppendLine("    [DisplayName(\"{0}\")]", item.Name);
            sb.AppendLine("    public {0} {1} {{ get; set; }}", type, item.Id);
        }
    }

    private static string GetCSharpType(FieldInfo item)
    {
        if (item.Type == FieldType.CheckBox || item.Type == FieldType.Switch)
            return "bool";
        else if (item.Type == FieldType.Date || item.Type == FieldType.DateTime)
            return "DateTime";
        else if (item.Type == FieldType.Number)
            return string.IsNullOrWhiteSpace(item.Length) ? "int" : "decimal";

        return "string";
    }
}