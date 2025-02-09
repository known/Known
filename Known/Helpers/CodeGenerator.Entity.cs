namespace Known.Helpers;

partial class CodeGenerator
{
    public string GetEntity(EntityInfo entity)
    {
        if (entity == null)
            return string.Empty;

        var sb = new StringBuilder();
        sb.AppendLine("namespace {0}.Entities;", Config.App.Id);
        sb.AppendLine(" ");
        sb.AppendLine("/// <summary>");
        sb.AppendLine("/// {0}实体类。", entity.Name);
        sb.AppendLine("/// </summary>");
        sb.AppendLine("public partial class {0} : {1}", entity.Id, entity.IsFlow ? "FlowEntity" : "EntityBase");
        sb.AppendLine("{");

        var index = 0;
        foreach (var item in entity.Fields)
        {
            if (index++ > 0)
                sb.AppendLine(" ");

            var type = GetCSharpType(item);
            if (!item.Required && type != "string")
                type += "?";

            sb.AppendLine("    /// <summary>");
            sb.AppendLine("    /// 取得或设置{0}。", item.Name);
            sb.AppendLine("    /// </summary>");
            sb.AppendLine("    [DisplayName(\"{0}\")]", item.Name);
            if (item.Required)
                sb.AppendLine("    [Required]");
            if (!string.IsNullOrWhiteSpace(item.Length) && type == "string")
                sb.AppendLine("    [MaxLength({0})]", item.Length);
            sb.AppendLine("    public {0} {1} {{ get; set; }}", type, item.Id);
        }
        sb.AppendLine("}");
        return sb.ToString();
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