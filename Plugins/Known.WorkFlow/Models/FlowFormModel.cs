namespace Known.WorkFlow.Models;

class FlowFormModel(BaseComponent component) : FormModel<FlowFormInfo>(component, true)
{
    internal void AddUserColumn(string id, string category)
    {
        AddRow().AddColumn(c => c.User, c =>
        {
            c.Id = id;
            c.Required = true;
            c.Type = FieldType.Custom;
            c.CustomField = nameof(UserPicker);
        });
    }

    internal void AddVerifyColumn()
    {
        AddRow().AddColumn(c => c.BizStatus, c =>
        {
            c.Id = "VerifyResult";
            c.Required = true;
            c.Type = FieldType.RadioList;
            c.Category = $"{FlowStatus.VerifyPass},{FlowStatus.VerifyFail}";
        });
    }

    internal void AddNoteColumn()
    {
        AddRow().AddColumn(c => c.Note, c =>
        {
            c.Id = "Note";
            c.Type = FieldType.TextArea;
        });
    }

    internal void AddReasonColumn(string name)
    {
        var reason = Language?["XXReason"]?.Replace("{name}", name);
        AddRow().AddColumn(c => c.Note, c =>
        {
            c.Name = reason;
            c.Required = true;
            c.Type = FieldType.TextArea;
        });
    }
}