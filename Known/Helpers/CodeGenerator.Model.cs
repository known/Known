namespace Known.Helpers;

partial class CodeGenerator
{
    public string GetModel(EntityInfo entity)
    {
        if (entity == null)
            return string.Empty;

        var modelName = entity.ModelName ?? entity.Id;
        var sb = new StringBuilder();
        sb.AppendLine("namespace {0}.Models;", entity.Namespace);
        sb.AppendLine(" ");
        sb.AppendLine("/// <summary>");
        sb.AppendLine("/// {0}信息类。", entity.Name);
        sb.AppendLine("/// </summary>");
        sb.AppendLine("[DisplayName(\"{0}\")]", entity.Name);
        sb.AppendLine("public class {0}", modelName);
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
            if (item.IsKey)
                sb.AppendLine("    [Required, Key]");
            else if (item.Required)
                sb.AppendLine("    [Required]");
            if (!string.IsNullOrWhiteSpace(item.Length) && type == "string")
                sb.AppendLine("    [MaxLength({0})]", item.Length);
            if (item.IsGrid)
                sb.AppendLine("    [Column]");
            if (item.IsForm)
                sb.AppendLine("    [Form]");
            sb.AppendLine("    [DisplayName(\"{0}\")]", item.Name);
            sb.AppendLine("    public {0} {1} {{ get; set; }}", type, item.Id);
        }
        sb.AppendLine("}");
        return sb.ToString();
    }
}