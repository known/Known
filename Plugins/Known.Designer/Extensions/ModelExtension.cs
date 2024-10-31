namespace Known.Designer.Extensions;

static class ModelExtension
{
    #region Entity
    internal static List<FieldInfo> GetFields(this EntityInfo info, Language language)
    {
        var infos = new List<FieldInfo>();
        if (info == null)
            return infos;

        foreach (var field in info.Fields)
        {
            infos.Add(new FieldInfo { Id = field.Id, Name = field.Name, Type = field.Type, Required = field.Required });
        }

        if (info.IsFlow)
        {
            infos.Add(GetFieldInfo(language, nameof(FlowEntity.BizStatus), FieldType.Text));
            infos.Add(GetFieldInfo(language, nameof(FlowEntity.ApplyBy), FieldType.Text));
            infos.Add(GetFieldInfo(language, nameof(FlowEntity.ApplyTime), FieldType.DateTime));
            infos.Add(GetFieldInfo(language, nameof(FlowEntity.VerifyBy), FieldType.Text));
            infos.Add(GetFieldInfo(language, nameof(FlowEntity.VerifyTime), FieldType.DateTime));
            infos.Add(GetFieldInfo(language, nameof(FlowEntity.VerifyNote), FieldType.Text));
        }

        infos.Add(GetFieldInfo(language, nameof(EntityBase.CreateBy), FieldType.Text));
        infos.Add(GetFieldInfo(language, nameof(EntityBase.CreateTime), FieldType.DateTime));
        infos.Add(GetFieldInfo(language, nameof(EntityBase.ModifyBy), FieldType.Text));
        infos.Add(GetFieldInfo(language, nameof(EntityBase.ModifyTime), FieldType.DateTime));

        return infos;
    }

    private static FieldInfo GetFieldInfo(Language language, string id, FieldType type) => new() { Id = id, Name = language[id], Type = type };
    #endregion

    #region ActionInfo
    internal static bool HasType(this ActionInfo info, string type) => !string.IsNullOrWhiteSpace(info.Position) && info.Position.Contains(type);
    #endregion
}