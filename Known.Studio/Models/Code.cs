namespace Known.Studio.Models;

class DomainInfo
{
    public DomainInfo(string model)
    {
        Fields = new List<FieldInfo>();
        ParseModel(model);
    }

    public string Project { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string Prefix { get; set; }
    public List<FieldInfo> Fields { get; set; }

    public string EntityName
    {
        get { return $"{Prefix}{Code}"; }
    }

    private void ParseModel(string model)
    {
        if (string.IsNullOrWhiteSpace(model))
            return;

        var lines = model.Split(Environment.NewLine.ToArray())
                         .Where(s => !string.IsNullOrWhiteSpace(s))
                         .ToArray();

        if (lines.Length > 0)
        {
            var values = lines[0].Split('|');
            if (values.Length > 0)
                Project = values[0];
            if (values.Length > 1)
                Prefix = values[1];
        }

        if (lines.Length > 1)
        {
            var values = lines[1].Split('|');
            if (values.Length > 0)
                Name = values[0];
            if (values.Length > 1)
                Code = values[1];
        }

        if (lines.Length > 2)
        {
            for (int i = 2; i < lines.Length; i++)
            {
                var field = new FieldInfo();
                var values = lines[i].Split('|');
                if (values.Length > 0)
                    field.Name = values[0];
                if (values.Length > 1)
                    field.Code = values[1];
                if (values.Length > 2)
                    field.Type = values[2];
                if (values.Length > 3)
                    field.Length = values[3];
                if (values.Length > 4)
                    field.Required = values[4] == "Y";
                if (values.Length > 5)
                    field.IsQuery = values[5] == "Y";

                if (field.Type == "CheckBox")
                {
                    field.Length = "50";
                    field.Required = true;
                }

                Fields.Add(field);
            }
        }
    }
}

class FieldInfo
{
    public string Code { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public string Length { get; set; }
    public bool Required { get; set; }
    public bool IsQuery { get; set; }
}