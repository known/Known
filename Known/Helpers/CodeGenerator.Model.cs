namespace Known.Helpers;

partial class CodeGenerator
{
    public string GetModel(EntityInfo entity)
    {
        if (entity == null)
            return string.Empty;

        var modelName = Model.ModelName;
        if (string.IsNullOrWhiteSpace(modelName))
            modelName = entity.Id;

        var sb = new StringBuilder();
        sb.AppendLine("namespace {0}.Models;", Model.Namespace);
        sb.AppendLine(" ");
        sb.AppendLine("/// <summary>");
        sb.AppendLine("/// {0}信息类。", entity.Name);
        sb.AppendLine("/// </summary>");
        sb.AppendLine("[DisplayName(\"{0}\")]", entity.Name);
        sb.AppendLine("public class {0}", modelName);
        sb.AppendLine("{");
        sb.AppendLine("    /// <summary>");
        sb.AppendLine("    /// 取得或设置ID。");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine("    public string Id { get; set; }");
        sb.AppendLine(" ");
        AppendFields(sb, entity, true);
        sb.AppendLine("}");
        return sb.ToString().TrimEnd([.. Environment.NewLine]);
    }
}